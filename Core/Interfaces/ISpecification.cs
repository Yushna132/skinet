using System;
using System.Linq.Expressions;

namespace Core.Interfaces;

public interface ISpecification<T>
{
    //C’est la condition de filtrage :
    //- `Func<Product, bool>` → une fonction qui prend un `Product` et retourne `true/false`.
    //Exemple : `p => p.Price > 100`

    //Mais attention : ici ce n’est pas un Func direct, c’est un Expression<Func<Product,bool>>
    //Expression = arbre d’expression que EF Core peut analyser et traduire en SQL (WHERE Price > 100).
    //C’est exactement ce que fait ton Specification Pattern → stocker ce Expression<Func<T,bool>>


    //Pourquoi get; uniquement (pas de set;)
    //Une spécification est censée être immuable :
    // Quand tu la crées, tu lui donnes un critère (p => p.Brand == "Nike").
    //Ensuite, ce critère ne doit pas changer → sinon, la requête ne serait plus fiable/testable.
    //En mettant seulement get;, tu dis :
    // "Une fois construit, je peux lire le critère mais pas le modifier"

    Expression<Func<T, bool>>? Criteria { get; }


    //Ici on a utiliser object mais pas bool pour le order by parce que si on regarde
    // dans ProductRepository pour order,pour la methode order comme entrée il prend decimal ou string
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDesc { get; }

    bool isDistinct { get; }
}

//on a implementé cette interface parce qu'on retourner un objet T non pas un string
public interface ISpecification<T, TResult> : ISpecification<T>
{
    Expression<Func<T, TResult>>? Select { get; }
}
