using LUMS.Core.Dtos;
using LUMS.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LUMS.Web.Controllers.Api;

[ApiController]
[ApiExceptionFilter]
[Route("api/books")]
[Tags("Books")]
public class BooksApiController : ControllerBase
{
    private readonly IBookService _books;

    public BooksApiController(IBookService books)
    {
        _books = books;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<BookSummaryDto>>> Search([FromQuery] BookSearchCriteria criteria, CancellationToken ct) =>
        Ok(await _books.SearchAsync(criteria, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDetailDto>> GetById(int id, CancellationToken ct) =>
        Ok(await _books.GetDetailAsync(id, ct));

    [HttpGet("filters")]
    public async Task<ActionResult<BookFilterOptionsDto>> GetFilters(CancellationToken ct) =>
        Ok(await _books.GetFilterOptionsAsync(ct));
}
