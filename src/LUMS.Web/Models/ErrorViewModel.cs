namespace LUMS.Web.Models;

public class ErrorViewModel
{
    public int Code { get; init; } = 500;

    public string? RequestId { get; init; }

    public string Heading => Code switch
    {
        404 => "Không tìm thấy trang",
        403 => "Không có quyền truy cập",
        _ => "Đã xảy ra lỗi"
    };

    public string Message => Code switch
    {
        404 => "Trang bạn tìm không tồn tại hoặc đã bị gỡ.",
        403 => "Tài khoản của bạn không có quyền xem trang này. Vui lòng đăng nhập với tài khoản phù hợp (Admin / Thủ thư / Độc giả).",
        _ => "Hệ thống gặp sự cố ngoài dự kiến. Vui lòng thử lại sau."
    };
}
