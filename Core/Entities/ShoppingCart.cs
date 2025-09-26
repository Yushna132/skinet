using System;
using System.Reflection;

namespace Core.Entities;

public class ShoppingCart
{
    public required string Id { get; set; }
    public List<CartItem> Items { get; set; } = [];
    // Ajouts pour Stripe
    public int? DeliveryMethodId { get; set; } // Méthode de livraison choisie
    public string? ClientSecret { get; set; } // Secret Stripe (client)
    public string? PaymentIntentId { get; set; } // ID de l’intention de paiement
}
