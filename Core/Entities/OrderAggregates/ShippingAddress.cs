using System;

namespace Core.Entities.OrderAggregates;

/* C’est un Owned Entity (entité possédée) : pas de BaseEntity, pas d’ID.
Elle sera stockée dans la même table que Order avec des colonnes distinctes. */
public class ShippingAddress
{
    public required string Name { get; set; }
    public required string Line1 { get; set; } 
    public string? Line2 { get; set; }
    public required string City { get; set; } 
    public required string State { get; set; } 
    public required string PostalCode { get; set; }
    public required string Country { get; set; }
}
