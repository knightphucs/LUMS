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

    // POST /Staff/UpdateBook  (FR-02-03) — form gửi id + các trường của UpdateBookRequest.
    [HttpPost]
    public async Task<IActionResult> UpdateBook(int id, UpdateBookRequest request, CancellationToken ct)
    {
        try
        {
            await _books.UpdateBookAsync(id, request, ct);
            TempData["Flash"] = $"Đã cập nhật đầu sách #{id} \"{request.Title.Trim()}\".";
        }
        catch (Exception ex) when (ex is DomainException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Books));
    }

    // POST /Staff/MergeCopies  (FR-02-03) — "Đồng ý gộp" khi thêm sách trùng ISBN: thêm bản sao vào đầu sách cũ.
    [HttpPost]
    public async Task<IActionResult> MergeCopies(int bookId, int copyCount, CancellationToken ct)
    {
        try
        {
            var created = await _books.AddCopiesAsync(bookId, copyCount, ct);
            TempData["Flash"] = $"Đã gộp thêm {created.Count} bản sao vào đầu sách #{bookId} ({created[0]} … {created[^1]}).";
        }
        catch (Exception ex) when (ex is DomainException or NotFoundException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Books));
    }
}
