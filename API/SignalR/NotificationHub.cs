using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class NotificationHub : Hub
{
    //connection id not email address
    //Vue qu'on a une seule serveur, on va utiliser un dictionary mais pour une plus grand nombre
    //de serveur, il est préférable d'utiliser Reedis

    //ConcurrentDictionary<email address,connection id>
    private static readonly ConcurrentDictionary<string, string> userConnectionsDictionary = new();

    public override Task OnConnectedAsync()
    {
        var email = Context.User?.GetEmail();
        if (!string.IsNullOrEmpty(email)) userConnectionsDictionary[email] = Context.ConnectionId;
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var email = Context.User?.GetEmail();
        if (!string.IsNullOrEmpty(email)) userConnectionsDictionary.TryRemove(email, out _);
        return base.OnDisconnectedAsync(exception);
    }

    public static string? GetConnectionIdByEmail(string email)
    {
        userConnectionsDictionary.TryGetValue(email, out var connectionId);
        return connectionId;
    }



}
