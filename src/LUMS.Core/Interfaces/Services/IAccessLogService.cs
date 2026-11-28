namespace LUMS.Core.Interfaces.Services;

/// <summary>Ghi nhật ký truy cập trái phép vào SystemLog (FR-00-07, dữ liệu hiển thị ở FR-03-04).</summary>
public interface IAccessLogService
{
    /// <summary>
    /// Ghi một dòng "Truy cập trái phép: {path}" cho tài khoản <paramref name="accountId"/> tại thời điểm hiện tại (IClock.Now)
    /// và lưu ngay. Gọi từ Program.cs → cookie auth OnRedirectToAccessDenied (cả trang web lẫn /api).
    /// </summary>
    /// <param name="accountId">Mã tài khoản đang đăng nhập nhưng không đủ quyền.</param>
    /// <param name="path">Đường dẫn bị từ chối, gồm query string (ví dụ "/Admin/Dashboard").</param>
    Task LogUnauthorizedAccessAsync(int accountId, string path, CancellationToken ct = default);
}
