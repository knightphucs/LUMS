using LUMS.Core.Enums;

namespace LUMS.Web.Security;

public static class RoleHome
{
    public static string UrlFor(string? role) => role switch
    {
        nameof(Role.Admin) => "/Admin/Dashboard",
        nameof(Role.ThuThu) => "/Staff/Books",
        _ => "/Books"
    };
}
