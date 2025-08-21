using System;

namespace API.Errors;

public class ApiErrorResponse(int statusCode, string message, string? details)
{
    public int StatusCode { get; set; } = statusCode;
    public string Message { get; set; } = message;
    //on a mis ? parceque en mode de production on n'a pas envi d'afficher les détailles
    public string? Details { get; set; } = details;

}
