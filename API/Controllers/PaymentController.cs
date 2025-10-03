using System;
using System.Linq.Expressions;
using API.Extensions;
using API.SignalR;
using Core.Entities;
using Core.Entities.OrderAggregates;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers;
//Injecter IHubContext<NotificationHub> pour pouvoir envoyer un message depuis l’API.
public class PaymentController(IPaymentService paymentService,
    IUnitOfWork unit, ILogger<PaymentController> logger,
    IConfiguration configuration,
    IHubContext<NotificationHub> hubContext) : BaseAPIController
{
    private readonly string _whSecret = configuration["StripeSettings:WhSecret"]!;
    //Créer ou mettre à jour le payment Intent
    [Authorize]
    [HttpPost("{cartId}")]
    public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
    {
        var shoppingCart = await paymentService.CreateOrUpdatePaymentIntent(cartId);

        if (shoppingCart == null) return BadRequest(new { Message = "There is a problem with your cart" });

        return Ok(shoppingCart);
    }



    //Obtenir toutes les méthodes de livraison
    [HttpGet("delivery-methods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethod()
    {
        var methods = await unit.Repository<DeliveryMethod>().ListAllAsync();
        return Ok(methods);
    }

    /* Le flux vu comme un mini algorithme
    reçoit requête POST /payments/webhook
        └─ lire JSON brut
        └─ vérifier signature avec WebhookSecret
            └─ si signature invalide → 400/500
        └─ extraire PaymentIntent
        └─ si intent.status == "succeeded":
            └─ retrouver Order via PaymentIntentId
            └─ comparer montant attendu (total*100) vs reçu
            └─ mettre OrderStatus = PaymentReceived ou PaymentMismatch
            └─ sauvegarder
        └─ répondre 200 OK à Stripe

    À retenir
    Pourquoi lire le corps “brut” ? → parce que Stripe signe le JSON tel quel.
    Pourquoi vérifier la signature ? → pour être sûr que c’est bien Stripe.
    Pourquoi convertir en cents ? → Stripe travaille en cents (ex: 228 € = 22800).
    Pourquoi chercher par PaymentIntentId ? → la commande et le paiement sont liés par cet ID.
    Pourquoi répondre 200 ? → pour dire à Stripe que tout est OK (sinon il retente). */

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync(); //On récupère le JSON brut envoyé par Stripe.
        try
        {
            /* ConstructStripeEvent appelle EventUtility.ConstructEvent 
            avec l’en-tête Stripe-Signature + ton Webhook Secret → si ça passe, l’événement est authentique. */

            var stripeEvent = ConstructStripeEvent(json); //vérifie la signature + parse l’événement

            if (stripeEvent.Data.Object is not PaymentIntent intent) //Si le paiement est réussi → mettre à jour la commande
            {
                return BadRequest("Invalid event data");
            }
            await HandlePaymentIntentSucceeded(intent);

            return Ok();
        }
        catch (StripeException ex)
        {

            logger.LogError(ex, "Stripe WebHook error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Stripe WebHook error");
        }
        catch (Exception ex)
        {

            logger.LogError(ex, "An unexpected error occured");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occured");
        }
    }

    /* On cherche la commande par PaymentIntentId, on compare le montant Stripe (en cents) 
    avec le total de la commande (converti en cents), puis on met à jour le statut et on sauve. */
    private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
    {
        if (intent.Status == "succeeded")
        {
            var spec = new OrderSpecification(intent.Id, true);
            var order = await unit.Repository<Order>().GetEntityWithSpec(spec)
                        ?? throw new Exception("Order not found");

            if ((long)order.GetTotal() * 100 != intent.Amount) //comparaison de montants
            {
                order.OrderStatus = OrderStatus.PaymentMismatch;
            }
            else
            {
                order.OrderStatus = OrderStatus.PaymentReceived;
            }

            await unit.Complete();

            /*Contexte: 
            Quand Stripe confirme un paiement (PaymentIntent Succeeded), 
            on met à jour la commande (order) dans la base de données.
            Ensuite, on veut notifier le client en temps réel que son paiement est bien reçu.
            C’est là qu’intervient SignalR. */
            /* NotificationHub est notre Hub SignalR (classe côté serveur).
            On a créé une méthode statique GetConnectionIdByEmail(email) → elle retourne le ConnectionId que SignalR a attribué à l’utilisateur lorsqu’il s’est connecté.
            Ici, on cherche l’utilisateur en fonction de son email de commande (order.BuyerEmail).
            Cela nous permet de savoir à quel navigateur correspond cet utilisateur. */
            var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);

            if (!string.IsNullOrEmpty(connectionId))
            {
                /* Ici, c’est le cœur de l’interaction API → Hub → Client : hubContext.Clients.Client(connectionId)
                hubContext est injecté dans le contrôleur (IHubContext<NotificationHub>).
                Clients.Client(connectionId) cible un client précis connecté à SignalR (celui qui a ce connectionId).
                Donc on envoie le message uniquement au client qui a payé, pas à tout le monde.
                .SendAsync("OrderCompleteNotification", order.ToDto())
                "OrderCompleteNotification" : c’est le nom de l’événement que le client Angular doit écouter.
                order.ToDto() : on envoie en plus l’objet Order DTO (données de la commande mise à jour). */
                await hubContext.Clients.Client(connectionId).SendAsync("OrderCompleteNotification", order.ToDto());
            }
        }
    }

    /* Valide la signature → protège ton endpoint.
    En cas d’échec, tu lances une StripeException */
    private Event ConstructStripeEvent(string json)
    {
        try
        {
            return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to construct stripe event");
            throw new StripeException("Invalid Signature");
        }
    }
}
