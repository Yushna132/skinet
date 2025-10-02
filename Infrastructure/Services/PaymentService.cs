using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class PaymentService(IConfiguration config, ICartService cartService,
        IUnitOfWork unit) : IPaymentService
{

    /* IConfiguration config
     Permet d’accéder à la configuration de l’application (appsettings.json). */

    /* ICartService cartService
        Sert à accéder et manipuler le panier de l’utilisateur.
    Utilisation :
        Récupérer le panier par son Id.
        Vérifier que le panier existe (null sinon).
        Sauvegarder les mises à jour (ajout PaymentIntentId, ClientSecret).
    Importance :
        Le panier est la source de vérité côté backend.
        On ne fait pas confiance aux données envoyées directement par le client.
        On s’assure que Stripe reflète bien l’état actuel du panier. */

    /* IGenericRepository<Product> productRepo
        Accès générique aux produits en base de données.
    Utilisation :
        Vérifier que chaque produit du panier existe encore.
        Mettre à jour le prix du panier si le prix du produit a changé en BDD.
    Importance :
        Anti-fraude → empêche un client de modifier le prix côté front.
        Garantit que le montant envoyé à Stripe = prix réel en BDD. */

    /* IGenericRepository<DeliveryMethod> dmRepo
        Accès aux méthodes de livraison.
    Utilisation :
        Vérifier que la méthode de livraison choisie existe.
        Récupérer son prix pour l’inclure dans le total Stripe.
    Importance :
        Empêche qu’un utilisateur passe une livraison gratuite fictive.
        
        Le montant payé inclut toujours livraison + produits validés par le serveur. */

    /* Le but de cette Methode */
    /* Lorsqu'un panier est passé au service
        -> Stripe crée un PaymentIntent si nouvelle.
        -> Stripe met a jour la PaymentIntent si le panier a changé
        Le panier en BDD contient desormais PaymentIntentId et ClientSecret
        Le front pourra utiliser ClientSecret pour déclencher le paiement */
    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {

        StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
        decimal shippingPrice = 0m;

        //Validating the cart
        var cart = await cartService.GetCartAsync(cartId);

        if (cart == null) return null; //si le panier est vide on retourne null

        //On verifie si il y un delivery Id associé au panier. 
        // On utilise le mot-clé HasValue parceque on a déclarer DeliveryMethodId comme ?
        // Vérifier la méthode de livraison
        if (cart.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync((int)cart.DeliveryMethodId);
            if (deliveryMethod == null) return null;

            shippingPrice = deliveryMethod.Price;
        }

        // Vérifier & mettre à jour les prix des produits
        foreach (var item in cart.Items)
        {
            var product = await unit.Repository<Core.Entities.Product>().GetByIdAsync(item.ProductId); // On cherche le produit dans le panier dans notre BDD(table product)
            if (product == null) return null;

            //On verifie le prix et on corrige
            if (item.Price != product.Price)
            {
                item.Price = product.Price;
            }
        }

        //Création // Mise à jour PaymentIntent
        var paymentService = new PaymentIntentService();
        PaymentIntent? intent = null; // On déclare une paymentIntent

        //On vérifie si on doit crée un paymentIntent ou pas
        // si c'est null on doit crée un paymentIntent
        if (string.IsNullOrEmpty(cart.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)cart.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)shippingPrice*100,
                Currency = "usd",
                PaymentMethodTypes = ["card"]
            };

            intent = await paymentService.CreateAsync(options); //Creation de notre paymentIntent
            cart.PaymentIntentId = intent.Id; //on ajoute le payment id au intent
            cart.ClientSecret = intent.ClientSecret;
        }
        else
        {
            //si le paymentIntent exist deja => On recalcule le prix
            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long)cart.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)shippingPrice*100
            };
            //mis à jour du intent
            intent = await paymentService.UpdateAsync(cart.PaymentIntentId, options);
        }

        //Sauveguarder panier mis à jour
        await cartService.SetCartAsync(cart);
        return cart;
    }
}
