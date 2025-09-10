using System;
using Core.Entities;

namespace Core.Interfaces;

public interface ICartService
{
    Task<ShoppingCart?> GetCartAsync(string cartId);       // Récupérer un panier
    Task<ShoppingCart?> SetCartAsync(ShoppingCart cart);   // Créer / Mettre à jour un panier
    Task<bool> DeleteCartAsync(string cartId);             // Supprimer un panier
}
