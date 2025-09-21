using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ShitChat.Shared.Extensions;

public static class HttpContextAccessorExtensions
{
    public static string GetUserId(this IHttpContextAccessor accesor)
    {
        var user = accesor.HttpContext.User
            ?? throw new InvalidOperationException("No HttpContext or User available");

        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID claim missing");
    }
}
