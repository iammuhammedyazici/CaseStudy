using System.Security.Claims;

namespace ECommerce.Order.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? user.FindFirst("sub")?.Value 
            ?? throw new UnauthorizedAccessException("User ID not found in token");
    }

    public static string GetUserRole(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value 
            ?? user.FindFirst("role")?.Value 
            ?? "Customer";
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.GetUserRole() == "Admin";
    }

    public static string GetUserEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value 
            ?? user.FindFirst("email")?.Value 
            ?? string.Empty;
    }

    public static string GetUserName(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value 
            ?? user.FindFirst("name")?.Value 
            ?? "Unknown";
    }
}
