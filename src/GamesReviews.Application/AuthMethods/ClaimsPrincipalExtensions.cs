using System.Security.Claims;

namespace GamesReviews.Application.AuthMethods;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst("userid")?.Value;

        return userIdClaim is null
            ? throw new UnauthorizedAccessException("User ID claim not found.")
            : Guid.Parse(userIdClaim);
    }

    public static string GetRole(this ClaimsPrincipal user)
    {
        var role = user.FindFirst(ClaimTypes.Role)?.Value;

        return role ?? throw new UnauthorizedAccessException("Role claim not found.");
    }
}