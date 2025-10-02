using System;
using System.Security.Cryptography.X509Certificates;
using Core.Entities.OrderAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    /* C’est une interface fournie par Entity Framework Core.
     Elle sert à configurer une entité (T) dans ta base de données, 
     sans surcharger ton DbContext */
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        /* Méthode d’Entity Framework Core pour configurer une owned entity (entité possédée).
        Une owned entity n’a pas sa propre table → ses colonnes sont stockées dans la table 
        du propriétaire. */
        builder.OwnsOne(x => x.ShippingAddress, o => o.WithOwner());
        builder.OwnsOne(x => x.PaymentSummary, o => o.WithOwner());

        /* Par défaut, EF stocke les enums comme int.
        Ici, on veut les stocker en string. */
        builder.Property(x => x.OrderStatus).HasConversion(
            o => o.ToString(),  // Enum → String (sauvegarde)
            o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o) // String → Enum (lecture)
        );

        /* Pour éviter les problèmes d’arrondi SQL, on précise decimal(18,2) */
        builder.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");

        /* Une commande peut avoir plusieurs items.
        Si on supprime une commande, on supprime aussi ses items (cascade delete). */
        builder.HasMany(x => x.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);

        /* - SQL Server ne stocke pas les `DateTime` comme UTC.
         On force la conversion pour éviter les incohérences. */
        builder.Property(x => x.OrderDate).HasConversion(
            d => d.ToUniversalTime(),
            d => DateTime.SpecifyKind(d, DateTimeKind.Utc)
        );
    }
}
