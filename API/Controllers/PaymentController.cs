using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PaymentController(IPaymentService paymentService, IUnitOfWork unit ) : BaseAPIController
{

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
        var methods = await  unit.Repository<DeliveryMethod>().ListAllAsync();
        return Ok(methods);
    }
}
