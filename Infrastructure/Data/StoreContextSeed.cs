using System;
using System.Reflection;
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
        /* En dev, on utilisait un chemin relatif (`../Infrastructure/...`), mais en prod la structure est différente.
        Solution : utiliser `Assembly.GetExecutingAssembly().Location` pour construire un chemin valide dans les deux cas. */
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!context.Products.Any())
        {
            //var productsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
            var productsData = await File.ReadAllTextAsync(path + @"/Data/SeedData/products.json");
            //convert the productsData into Product object(fount in Entities);
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);

            if (products == null) return;

            //adding products in the table Products in our db
            context.Products.AddRange(products);

            await context.SaveChangesAsync();

        }

         if (!context.DeliveryMethods.Any())
        {
            //var deliveryData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/delivery.json");
            var deliveryData = await File.ReadAllTextAsync(path + @"/Data/SeedData/delivery.json");
            //convert the deliveryData into DeliveryMethod object(fount in Entities);
            var delivery = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryData);

            if (delivery == null) return;

            //adding delivery in the table DeliveryMethod in our db
            context.DeliveryMethods.AddRange(delivery);

            await context.SaveChangesAsync();
        }

    }

}
