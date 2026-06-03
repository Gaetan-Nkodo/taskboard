using System.Security.Claims;

namespace TaskBoard.Api.Extensions;

public static class ClaimsExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var idClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                   ?? user.Claims.FirstOrDefault(c => c.Type == "sub");

        if (idClaim is null)
            throw new InvalidOperationException("User id claim not found.");

        return Guid.Parse(idClaim.Value);
    }
}
