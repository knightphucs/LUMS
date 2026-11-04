namespace LUMS.Core.Entities;

public class PolicyConfiguration
{
    public int Id { get; set; }
    public int MaxLoanDays { get; set; }
    public int MaxBooksPerReader { get; set; }
    public int MaxReservations { get; set; }

    public decimal FinePerDay { get; set; }

    public int UpdatedById { get; set; }
    public Account UpdatedBy { get; set; } = null!;
    public DateTime EffectiveFrom { get; set; }

    public static PolicyConfiguration Default => new()
    {
        MaxLoanDays = 14,
        MaxBooksPerReader = 5,
        MaxReservations = 3,
        FinePerDay = 5000m
    };
}
