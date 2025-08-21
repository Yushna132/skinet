using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware;

public class ExceptionMiddleware(IHostEnvironment env, RequestDelegate next)
{
    //Un middleware personnalisé doit :
    // 1. Avoir un constructeur qui reçoit au minimum un RequestDelegate next.
    // 2. Avoir une méthode publique nommée InvokeAsync (ou Invoke) avec au moins un paramètre HttpContext.

    //Pourquoi InvokeAsync ?
    // Quand tu ajoutes app.UseMiddleware<MyMiddleware>(), ASP.NET Core réfléchit (via Reflection) pour trouver une méthode compatible.
    // Il cherche d’abord InvokeAsync(HttpContext, …), sinon il prend Invoke(HttpContext, …).
    // Si ni l’une ni l’autre n’existent → erreur : “Middleware MyMiddleware must contain an Invoke or InvokeAsync method.”

    //C’est la porte d’entrée obligatoire pour que ton code s’exécute dans le pipeline.
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, env);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex, IHostEnvironment env)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = env.IsDevelopment()
            ? new ApiErrorResponse(context.Response.StatusCode, ex.Message, ex.StackTrace)
            : new ApiErrorResponse(context.Response.StatusCode, ex.Message, "Internal server error");

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var json = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsync(json);
    }
}
