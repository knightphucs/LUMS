using LUMS.Core.Enums;

namespace LUMS.Core.Entities;

public class Copy
{
    public string Id { get; set; } = string.Empty;

    public int BookTitleId { get; set; }
    public BookTitle BookTitle { get; set; } = null!;

    public string Shelf { get; set; } = string.Empty;
    public CopyStatus Status { get; set; }
    public DateTime AddedAt { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
