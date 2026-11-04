using Microsoft.AspNetCore.Mvc.Rendering;

namespace LUMS.Web.Presentation;

public static class NavExtensions
{
    /// <summary>
    /// Trả về "is-active" khi trang hiện tại thuộc controller (và action, nếu truyền vào) — dùng cho các link menu trong layout.
    /// </summary>
    public static string ActiveClass(this ViewContext context, string controller, string? action = null)
    {
        var values = context.RouteData.Values;
        var isActive =
            string.Equals(values["controller"] as string, controller, StringComparison.OrdinalIgnoreCase) &&
            (action is null || string.Equals(values["action"] as string, action, StringComparison.OrdinalIgnoreCase));
        return isActive ? "is-active" : string.Empty;
    }
}
