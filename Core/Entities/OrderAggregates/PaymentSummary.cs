using System;

namespace Core.Entities.OrderAggregates;
/* Sauvegarde des infos techniques du paiement.
Pas relié à Stripe directement, mais conserve les données de la transaction. */
public class PaymentSummary
{
    public int Last4 { get; set; }
    public required string CardBrand { get; set; }
    public int ExpMonth { get; set; }
    public int ExpYear { get; set; }
}
