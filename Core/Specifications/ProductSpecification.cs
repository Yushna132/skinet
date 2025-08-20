using System;
using System.Security.Cryptography.X509Certificates;
using Core.Entities;

namespace Core.Specifications;

public class ProductSpecification : BaseSpecification<Product>
{
    //ProductSpecification
    // Ici, tu veux filtrer par brand et type → tu construis une expression 
    // dans ton constructeur et tu la passes à la BaseSpecification.
    // D’où l’appel à : base(p => …) qui initialise le Criteria.


    //Constructor

    public ProductSpecification(string? brand, string? type, string? sort) : base(p =>
        (string.IsNullOrWhiteSpace(brand) || p.Brand == brand) &&
        (string.IsNullOrWhiteSpace(type) || p.Type == type)
    )
    {
        switch (sort)
        {
            case "priceAsc":
                AddOrderBy(p => p.Price);
                break;

            case "priceDesc":
                AddOrderByDesc(p => p.Price);
                break;

            default:
                AddOrderBy(p => p.Name);
                break;

        }
    }

}
