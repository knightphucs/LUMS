using LUMS.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LUMS.Web.ViewComponents;

/// <summary>Lưới "Sách mới cập nhật" — đặt được ở bất kỳ trang nào mà không cần Controller nạp dữ liệu.</summary>
public class LatestBooksViewComponent : ViewComponent
{
    private readonly IBookService _books;

    public LatestBooksViewComponent(IBookService books)
    {
        _books = books;
    }

    // <vc:latest-books take="4" />
    public async Task<IViewComponentResult> InvokeAsync(int take = 4) =>
        View(await _books.GetLatestAsync(take, HttpContext.RequestAborted));
}
