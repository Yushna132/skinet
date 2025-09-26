using System;
using Core.Entities;

namespace Core.Interfaces;

public interface IPaymentService
{

    /* On renvoie null quand :
        le panier n’existe pas,
        la livraison est invalide,
        ou un produit n’existe plus. */
    /* On met ? pour signaler que le retour peut être null volontairement.
    Ça oblige le dev (toi ou quelqu’un d’autre) à gérer ce cas.
    Au lieu d’un crash imprévu (NullReferenceException), on a un flux contrôlé 
    (ex. retour BadRequest). */
    Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId);


}
