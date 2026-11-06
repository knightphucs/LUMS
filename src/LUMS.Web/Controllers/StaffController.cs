using LUMS.Core.Dtos;
using LUMS.Core.Exceptions;
using LUMS.Core.Interfaces.Services;
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

    // POST /Staff/CreateBook  (FR-02-03) — form gửi các trường của CreateBookRequest.
    [HttpPost]
    public async Task<IActionResult> CreateBook(CreateBookRequest request, CancellationToken ct)
    {
        try
        {
            var id = await _books.CreateBookAsync(request, ct);
            TempData["Flash"] = $"Đã thêm đầu sách #{id} \"{request.Title.Trim()}\" với {request.CopyCount} bản sao.";
            return RedirectToAction(nameof(Books));
        }
        catch (DomainException ex)
        {
            // Gồm cả DuplicateIsbnException: thông báo đã chứa mã đầu sách cũ.
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Books));
        }
    }
}
