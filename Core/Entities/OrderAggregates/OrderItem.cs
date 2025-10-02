using System;

namespace Core.Entities.OrderAggregates;
/* Entité avec ID (BaseEntity).
Contient le produit commandé + quantité + prix. */
public class OrderItem : BaseEntity
{
    public ProductItemOrdered ItemOrdered { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }

}
