namespace LUMS.Web.Models;

public record PagerViewModel(int Page, int TotalPages, string PagePath, IDictionary<string, object?> Query);
