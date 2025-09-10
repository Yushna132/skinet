using System;
using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class CartService(IConnectionMultiplexer redis) : ICartService
{
    //IConnectionMultiplexer (fourni par StackExchange.Redis) → permet de récupérer une connexion à Redis.
    //Depuis cette connexion : IDatabase _database = redis.GetDatabase();

    private readonly IDatabase _database = redis.GetDatabase();
    public async Task<bool> DeleteCartAsync(string cartId)
    {
        return await _database.KeyDeleteAsync(cartId);
    }

    public async Task<ShoppingCart?> GetCartAsync(string cartId)
    {
        var data = await _database.StringGetAsync(cartId);
        return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<ShoppingCart>(data!);
        
        /* Le ! dit au compilateur :
         « Je sais que data pourrait être null, mais fais-moi confiance, ici il ne l’est pas. »
        C’est donc une façon de désactiver l’avertissement de nullabilité que le compilateur génère 
        (car data est un RedisValue qui pourrait être vide/null). */
    }

    public async Task<ShoppingCart?> SetCartAsync(ShoppingCart cart)
    {
        var created = await _database.StringSetAsync(cart.Id, JsonSerializer.Serialize(cart), TimeSpan.FromDays(30));
        if (!created) return null;
        return await GetCartAsync(cart.Id);
    }
}
