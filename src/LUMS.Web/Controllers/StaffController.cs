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

    // Khoá ViewData mà _BookDialog.cshtml đọc để mở lại hộp thoại với dữ liệu đã nhập.
    public const string BookFormKey = "BookForm";
    public const string BookFormIdKey = "BookFormId";
    public const string MergeSuggestionKey = "MergeSuggestion";

    private readonly IBookService _books;

    public StaffController(IBookService books)
    {
        _books = books;
    }

    // GET /Staff/Books?keyword=&category=&status=&pageNumber=
    public async Task<IActionResult> Books(
        string? keyword, string? category, string? status, int pageNumber = 1, CancellationToken ct = default) =>
        View(await BuildBooksModelAsync(keyword, category, status, pageNumber, ct));

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
        catch (DuplicateIsbnException ex)
        {
            // Trùng ISBN → hỏi gộp thêm bản sao vào đầu sách cũ (POST MergeCopies).
            var suggestion = await _books.GetMergeSuggestionAsync(request.Isbn, ct);
            if (suggestion is null)
            {
                return await BookFormErrorAsync(request, null, ex, ct);
            }

            ViewData[BookFormKey] = request;
            ViewData[MergeSuggestionKey] = suggestion;
            return View(nameof(Books), await BuildBooksModelAsync(null, null, null, 1, ct));
        }
        catch (DomainException ex)
        {
            return await BookFormErrorAsync(request, null, ex, ct);
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
        catch (DomainException ex)
        {
            return await BookFormErrorAsync(request, id, ex, ct);
        }
        catch (NotFoundException ex)
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

    /// <summary>
    /// Trả lại trang danh sách và mở lại hộp thoại với dữ liệu vừa nhập; lỗi hiện dưới ô liên quan
    /// (hoặc ở đầu hộp thoại nếu không xác định được ô).
    /// </summary>
    private async Task<IActionResult> BookFormErrorAsync(UpdateBookRequest request, int? id, DomainException ex, CancellationToken ct)
    {
        ModelState.Clear(); // bỏ lỗi binding mặc định (tiếng Anh), chỉ giữ thông báo của service
        // Khi sửa, gợi ý "thêm bản sao" của DuplicateIsbnException không phù hợp → báo ngắn gọn.
        var message = id is not null && ex is DuplicateIsbnException dup
            ? $"ISBN {dup.Isbn} đã thuộc đầu sách #{dup.ExistingBookId}; không thể đổi sang ISBN của đầu sách khác."
            : ex.Message;
        ModelState.AddModelError(FieldOf(ex), message);
        ViewData[BookFormKey] = request;
        ViewData[BookFormIdKey] = id;
        return View(nameof(Books), await BuildBooksModelAsync(null, null, null, 1, ct));
    }

    /// <summary>Đoán ô bị lỗi từ thông báo của BookService (các thông báo đều mở đầu bằng tên trường).</summary>
    private static string FieldOf(DomainException ex)
    {
        if (ex is DuplicateIsbnException) return nameof(CreateBookRequest.Isbn);

        var map = new (string Prefix, string Field)[]
        {
            ("Tên sách", nameof(CreateBookRequest.Title)),
            ("Tác giả", nameof(CreateBookRequest.Author)),
            ("Thể loại", nameof(CreateBookRequest.Category)),
            ("Vị trí kệ", nameof(CreateBookRequest.ShelfLocation)),
            ("Mã ISBN", nameof(CreateBookRequest.Isbn)),
            ("Số lượng bản sao", nameof(CreateBookRequest.CopyCount))
        };
        return map.FirstOrDefault(m => ex.Message.StartsWith(m.Prefix, StringComparison.Ordinal)).Field ?? string.Empty;
    }

    private async Task<BookSearchViewModel> BuildBooksModelAsync(
        string? keyword, string? category, string? status, int pageNumber, CancellationToken ct)
    {
        var result = await _books.SearchAsync(new BookSearchCriteria
        {
            Keyword = keyword,
            Category = category,
            InStock = BookSearchViewModel.ToInStock(status),
            Page = pageNumber,
            PageSize = BooksPageSize
        }, ct);

        return new BookSearchViewModel
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
    }
}
