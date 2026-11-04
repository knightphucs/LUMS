using LUMS.Core.Enums;

namespace LUMS.Web.Security;

public static class AppRoles
{
    public const string Reader = nameof(Role.DocGia);
    public const string Librarian = nameof(Role.ThuThu);
    public const string Admin = nameof(Role.Admin);

    public const string Staff = Librarian + "," + Admin;
}
