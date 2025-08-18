using System;
using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    //*** Pourquoi faire la méthode static ***//
    // Static : nous pouvons utiliser cette méthode sans avoir à créer une nouvelle 
    // instance de la classe storecontextseed

    public static async Task SeedAsync(StoreContext context)
    {
        if (!context.Products.Any())
        {
            var productsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
            //convert the productsData into Product object(fount in Entities);
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);

            if (products == null) return;

            //adding products in the table Products in our db
            context.Products.AddRange(products);

            await context.SaveChangesAsync();

        }

    }

}
