namespace Core.Entities.OrderAggregates;
//Définit les différents états possibles de la commande.
public enum OrderStatus
{
    Pending,
    PaymentReceived,
    PaymentFailed,
    PaymentMismatch

}
