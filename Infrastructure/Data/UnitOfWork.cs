using System;
using System.Collections.Concurrent;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data;

public class UnitOfWork(StoreContext context) : IUnitOfWork
{
    private readonly ConcurrentDictionary<string, object> _respositories = new();
    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        context.Dispose();
    }

    //Donner accès à un dépôt générique pour n’importe quelle entité TEntity

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        // 1. On récupère le type demandé (par ex : "Order", "Product")
        var type = typeof(TEntity).Name;

        // 2. On regarde si le repo existe déjà dans le dictionnaire (_repositories)
        return (IGenericRepository<TEntity>)_respositories.GetOrAdd(type, t =>
        {
            // 3. Si non, on fabrique dynamiquement le type GenericRepository<TEntity>
            var repositoryType = typeof(GenericRepository<>).MakeGenericType(typeof(TEntity));

            // 4. On crée une instance avec Activator (équivaut à "new GenericRepository<TEntity>(context)")
            return Activator.CreateInstance(repositoryType, context)
                ?? throw new InvalidOperationException(
                    $"Could not create repository instance for {t}");
        });
    }
}
