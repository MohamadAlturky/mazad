using System.Security.Claims;

namespace Mazad.Core.Domain.Users.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        if (principal == null)
        {
            return 0;
        }

        var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
        {
            return userId;
        }

        return 0;
    }
}