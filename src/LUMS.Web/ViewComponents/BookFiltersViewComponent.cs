using LUMS.Core.Interfaces.Services;
using LUMS.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace LUMS.Web.ViewComponents;

/// <summary>
/// Các ô lọc Thể loại / Tác giả / Tình trạng dùng chung cho /Books và /Staff/Books.
/// Tự lấy danh sách lựa chọn từ IBookService nên Controller không phải nạp sẵn.
/// </summary>
public class BookFiltersViewComponent : ViewComponent
{
    private readonly IBookService _books;

    public BookFiltersViewComponent(IBookService books)
    {
        _books = books;
    }

    // <vc:book-filters category="..." author="..." status="..." staff="true" />
    public async Task<IViewComponentResult> InvokeAsync(
        string? category = null, string? author = null, string? status = null, bool staff = false)
    {
        var model = new BookFiltersViewModel
        {
            Options = await _books.GetFilterOptionsAsync(HttpContext.RequestAborted),
            Category = category,
            Author = author,
            Status = status,
            Staff = staff
        };
        return View(model);
    }
}
