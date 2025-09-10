using System;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace API.Controllers;

public class CartController(ICartService cartService) : BaseAPIController
{
    [HttpGet]
    public async Task<ActionResult<ShoppingCart>> GetCardById(string id)
    {
        var cart = await cartService.GetCartAsync(id);
        // Si pas trouvé, on retourne un panier vide (même Id) pour initialiser côté client
        return Ok(cart ?? new ShoppingCart { Id = id });
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart)
    {
        var updatedCart = await cartService.SetCartAsync(cart);
        if (updatedCart == null) return BadRequest("Problem with cart");
        return updatedCart;
    }

    public async Task<ActionResult> DeleteCart(string id)
    {
        var result = await cartService.DeleteCartAsync(id);
        if (!result) return BadRequest("Problem deleting cart");
        return Ok();
    }


    


}
