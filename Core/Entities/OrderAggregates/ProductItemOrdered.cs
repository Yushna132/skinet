using System;

namespace Core.Entities.OrderAggregates;

/* Sert de snapshot : même si le produit change plus tard (nom, image, prix…), 
on garde l’info au moment de l’achat.
    Aussi une Owned Entity. */
public class ProductItemOrdered
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string PictureUrl { get; set; }

}
