using System;
using System.Security.Cryptography.X509Certificates;
using Core.Entities.OrderAggregates;

namespace Core.Specifications;

public class OrderSpecification : BaseSpecification<Order>
{
    //Récupérer toutes les commandes d’un utilisateur (par email)
    public OrderSpecification(string email) : base(x => x.BuyerEmail == email)
    {
        AddInclude(x => x.OrderItems);
        AddInclude(x => x.DeliveryMethod);
        AddOrderByDesc(x => x.OrderDate);

    }

    // Récupérer une commande spécifique par Id + email utilisateur
    public OrderSpecification(string email, int id) : base(x => x.BuyerEmail == email && x.Id == id)
    {
        AddInclude("OrderItems");
        AddInclude("DeliveryMethod");
    }

    public OrderSpecification(string paymentIntentId, bool isPaymentIntent): base(x => x.PaymentIntentId == paymentIntentId)
    {
        AddInclude("OrderItems");
        AddInclude("DeliveryMethod");
    }

}
