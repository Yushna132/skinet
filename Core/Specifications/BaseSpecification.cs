using System;
using System.Linq.Expressions;
using System.Reflection;
using Core.Interfaces;

namespace Core.Specifications;

public class BaseSpecification<T>(Expression<Func<T, bool>>? criteria) : ISpecification<T>
{

    //Elle définit la structure commune pour toutes les spécifications (Criteria, Includes, OrderBy, etc.).
    //Elle expose un constructeur vide (protected BaseSpecification()) → utile si tu veux créer une spec sans critères.
    // Elle expose aussi un constructeur avec paramètre (Expression<Func<T,bool>> criteria) → pour directement définir un filtre.


    //empty constructor
    protected BaseSpecification() : this(null) { }
    public Expression<Func<T, bool>>? Criteria => criteria;

    public Expression<Func<T, object>>? OrderBy { get; private set; }

    public Expression<Func<T, object>>? OrderByDesc { get; private set; }

    public bool isDistinct { get; private set; }

    public int Take { get; private set; }


    public int Skip { get; private set; }


    public bool isPagingEnabled { get; private set; }

    public List<Expression<Func<T, object>>> Includes { get; } = [];
    public List<string> IncludeStrings { get; } = [];

    public IQueryable<T> ApplyCriteria(IQueryable<T> query)
    {
        if (Criteria != null)
        {
            query = query.Where(Criteria);
        }
        return query;
    }

    /* Ici tu définis la propriété Includes et une méthode AddInclude qui permet de rajouter des navigations à charger.
    Tu peux appeler AddInclude(x => x.OrderItems) dans une spécification concrète.
    Ça ajoute la lambda x => x.OrderItems dans la liste Includes.
    Ensuite, EF saura traduire ça en .Include(o => o.OrderItems).
    C’est du type-safe Include. */

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

     protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected void AddOrderByDesc(Expression<Func<T, object>> orderByDescExpression)
    {
        OrderByDesc = orderByDescExpression;
    }

    protected void ApplyDistinct()
    {
        isDistinct = true;
    }

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        isPagingEnabled = true;
    }

}

public class BaseSpecification<T, TResult>(Expression<Func<T, bool>>? criteria)
        : BaseSpecification<T>(criteria), ISpecification<T, TResult>
{
    protected BaseSpecification() : this(null) { }
    public Expression<Func<T, TResult>>? Select { get; private set; }

    protected void AddSelect(Expression<Func<T, TResult>> selectExpression)
    {
        Select = selectExpression;
    }

}
