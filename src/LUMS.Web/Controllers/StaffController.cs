using LUMS.Core.Dtos;
using LUMS.Core.Services.Interfaces;
using LUMS.Web.Models;
using LUMS.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LUMS.Web.Controllers;

[Authorize(Roles = AppRoles.Staff)]
public class StaffController : Controller
{
    private const int BooksPageSize = 10;

    private readonly IBookService _books;

    public StaffController(IBookService books)
    {
        _books = books;
    }

    // GET /Staff/Books?keyword=&category=&status=&pageNumber=
    public async Task<IActionResult> Books(
        string? keyword, string? category, string? status, int pageNumber = 1, CancellationToken ct = default)
    {
        var filters = await _books.GetFilterOptionsAsync(ct);
        var result = await _books.SearchAsync(new BookSearchCriteria
        {
            Keyword = keyword,
            Category = category,
            InStock = BookSearchViewModel.ToInStock(status),
            Page = pageNumber,
            PageSize = BooksPageSize
        }, ct);

        var model = new BookSearchViewModel
        {
            Keyword = keyword,
            Category = category,
            Status = status,
            Filters = filters,
            Result = result,
            Pager = new PagerViewModel(
                result.Page,
                result.TotalPages,
                "/Staff/Books",
                new Dictionary<string, object?>
                {
                    ["keyword"] = keyword,
                    ["category"] = category,
                    ["status"] = status
                })
        };
        return View(model);
    }
}
