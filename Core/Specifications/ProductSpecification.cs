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

    public ProductSpecification(ProductSpecParams specParams) : base(p =>
        (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search)) &&
        (specParams.Brands.Count == 0 || specParams.Brands.Contains(p.Brand)) &&
        (specParams.Types.Count == 0 || specParams.Types.Contains(p.Type))
    )
    {
        ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

        switch (specParams.Sort)
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
