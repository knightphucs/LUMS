using LUMS.Core.Enums;

namespace LUMS.Core.Entities;

public class Account
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public Role Role { get; set; }
    public ReaderType? ReaderType { get; set; }
    public AccountStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
