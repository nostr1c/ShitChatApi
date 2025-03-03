using System.Security.Claims;

namespace ShitChat.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserGuid(this ClaimsPrincipal user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);

        return idClaim?.Value;
    }
}
