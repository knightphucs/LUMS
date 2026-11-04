using LUMS.Core.Dtos;

namespace LUMS.Web.Models;

public class DashboardViewModel
{
    public AdminDashboardDto Data { get; init; } = null!;

    public PolicyForm Policy { get; init; } = new();

    public int BarPercent(MonthlyLoanCount month)
    {
        var max = Data.MonthlyLoans.Count == 0 ? 0 : Data.MonthlyLoans.Max(m => m.Count);
        return max == 0 ? 0 : (int)Math.Round(month.Count * 100.0 / max);
    }
}
