using LUMS.Core.Dtos;
using LUMS.Core.Exceptions;
using LUMS.Core.Services.Interfaces;
using LUMS.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace LUMS.Web.Controllers;

public class BooksController : Controller
{
    private const int PageSize = 8;

    private readonly IBookService _books;

    public BooksController(IBookService books)
    {
        _books = books;
    }

    // GET /Books?keyword=&category=&author=&status=&pageNumber=
    public async Task<IActionResult> Index(
        string? keyword, string? category, string? author, string? status, int pageNumber = 1, CancellationToken ct = default)
    {
        var filters = await _books.GetFilterOptionsAsync(ct);
        var result = await _books.SearchAsync(new BookSearchCriteria
        {
            Keyword = keyword,
            Category = category,
            Author = author,
            InStock = BookSearchViewModel.ToInStock(status),
            Page = pageNumber,
            PageSize = PageSize
        }, ct);

        var model = new BookSearchViewModel
        {
            Keyword = keyword,
            Category = category,
            Author = author,
            Status = status,
            Filters = filters,
            Result = result,
            Pager = new PagerViewModel(
                result.Page,
                result.TotalPages,
                "/Books",
                new Dictionary<string, object?>
                {
                    ["keyword"] = keyword,
                    ["category"] = category,
                    ["author"] = author,
                    ["status"] = status
                })
        };
        return View(model);
    }

    // GET /Books/Details/5
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        try
        {
            return View(await _books.GetDetailAsync(id, ct));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
