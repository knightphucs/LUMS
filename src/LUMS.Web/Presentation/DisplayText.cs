using LUMS.Core.Dtos;
using LUMS.Core.Enums;

namespace LUMS.Web.Presentation;

public static class DisplayText
{
    public const string Ok = "badge--ok";
    public const string Warn = "badge--warn";
    public const string Danger = "badge--danger";
    public const string Neutral = "badge--neutral";

    public static string RoleName(Role role) => role switch
    {
        Role.DocGia => "Độc giả",
        Role.ThuThu => "Thủ thư",
        Role.Admin => "Quản trị",
        _ => role.ToString()
    };

    public static string RoleBadge(Role role) => role switch
    {
        Role.Admin => Danger,
        Role.ThuThu => Ok,
        _ => Neutral
    };

    public static string RoleName(string? role) =>
        Enum.TryParse<Role>(role, out var r) ? RoleName(r) : string.Empty;

    public static string RoleBadge(string? role) =>
        Enum.TryParse<Role>(role, out var r) ? RoleBadge(r) : Neutral;

    public static string AccountStatusName(AccountStatus status) => status switch
    {
        AccountStatus.HoatDong => "Hoạt động",
        AccountStatus.Khoa => "Đã khóa",
        AccountStatus.ChoXacNhan => "Chờ xác nhận",
        _ => status.ToString()
    };

    public static string AccountStatusBadge(AccountStatus status) => status switch
    {
        AccountStatus.HoatDong => Ok,
        AccountStatus.Khoa => Danger,
        _ => Warn
    };

    public static string AvailabilityName(AvailabilityLevel level) => level switch
    {
        AvailabilityLevel.Available => "Còn sách",
        AvailabilityLevel.Low => "Sắp hết",
        _ => "Hết sách"
    };

    public static string AvailabilityBadge(AvailabilityLevel level) => level switch
    {
        AvailabilityLevel.Available => Ok,
        AvailabilityLevel.Low => Warn,
        _ => Danger
    };

    public static string CopyStatusName(CopyStatus status) => status switch
    {
        CopyStatus.SanSang => "Sẵn sàng",
        CopyStatus.DangDuocMuon => "Đang được mượn",
        CopyStatus.DangGiuCho => "Đang giữ chỗ",
        CopyStatus.Hong => "Hỏng",
        CopyStatus.Mat => "Mất",
        _ => status.ToString()
    };

    public static string CopyStatusBadge(CopyStatus status) => status switch
    {
        CopyStatus.SanSang => Ok,
        CopyStatus.DangDuocMuon or CopyStatus.DangGiuCho => Warn,
        _ => Danger
    };

    public static string ReservationName(ReservationStatus status, int queuePosition) => status switch
    {
        ReservationStatus.DangCho => $"Đang chờ (hạng {queuePosition})",
        ReservationStatus.SanSangNhanSach => "Sẵn sàng nhận sách",
        ReservationStatus.DaHuy => "Đã hủy",
        _ => "Đã hoàn tất"
    };

    public static string ReservationBadge(ReservationStatus status) => status switch
    {
        ReservationStatus.SanSangNhanSach => Ok,
        ReservationStatus.DangCho => Warn,
        _ => Neutral
    };

    public static string Money(decimal amount) =>
        string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:N0}", amount).Replace(',', '.') + "đ";

    public static string Date(DateOnly date) => date.ToString("dd/MM/yyyy");

    public static string DateTimeText(DateTime value) => value.ToString("dd/MM/yyyy HH:mm");
}
