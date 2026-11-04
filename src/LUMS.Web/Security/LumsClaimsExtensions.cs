using System.Security.Claims;

namespace LUMS.Web.Security;

public static class LumsClaimsExtensions
{
    public static int GetAccountId(this ClaimsPrincipal user) =>
        int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id)
            ? id
            : throw new InvalidOperationException("Người dùng chưa đăng nhập.");

    public static string GetFullName(this ClaimsPrincipal user) => user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

    public static string GetCode(this ClaimsPrincipal user) => user.FindFirst(LumsClaimTypes.Code)?.Value ?? string.Empty;

    public static string? GetRoleName(this ClaimsPrincipal user) => user.FindFirst(ClaimTypes.Role)?.Value;

    public static bool IsStaff(this ClaimsPrincipal user) =>
        user.IsInRole(AppRoles.Librarian) || user.IsInRole(AppRoles.Admin);

    public static string GetInitials(this ClaimsPrincipal user)
    {
        var parts = user.GetFullName().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => "?",
            1 => parts[0][..1].ToUpperInvariant(),
            _ => (parts[0][..1] + parts[^1][..1]).ToUpperInvariant()
        };
    }
}
