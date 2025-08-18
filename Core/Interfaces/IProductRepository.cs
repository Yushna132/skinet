using System;
using Core.Entities;

namespace Core.Interfaces;

public interface IProductRepository
{
    //IReadonly est utilisé pour dire qu'on ne va pas modifier la liste mais uniquement l'utiliser
    Task<IReadOnlyList<Product>> GetProductsAsync(string? brands, string? types, string? sort);
    Task<Product?> GetProductByIdAsync(int id);
    Task<IReadOnlyList<string>> GetBrandsAsync();
    Task<IReadOnlyList<string>> GetTypesAsync();

    // Methodes qui vont add/ update/ delete methode
    // C'est méthodes ne seront pas asynchronous
    // l'action est ajouter au Entity Framework tracking
    // On n'appelle pas la bd
    // c'est que lorsqu'on save le changes qu'on fait appel a bd
    void AddProduct(Product product);
    void UpdateProduct(Product product);
    void DeleteProduct(Product product);
    bool ProductExists(int id);
    Task<bool> SaveChangesAsync();








}
