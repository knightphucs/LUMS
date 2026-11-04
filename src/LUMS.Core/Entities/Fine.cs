using LUMS.Core.Enums;

namespace LUMS.Core.Entities;

public class Fine
{
    public int Id { get; set; }

    public int? LoanId { get; set; }
    public Loan? Loan { get; set; }

    public string? CopyId { get; set; }
    public Copy? Copy { get; set; }

    public int ReaderId { get; set; }
    public Account Reader { get; set; } = null!;

    public int LibrarianId { get; set; }
    public Account Librarian { get; set; } = null!;

    public FineType Type { get; set; }

    public decimal Amount { get; set; }

    public string? Note { get; set; }
    public DateTime HandledAt { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}
