using LUMS.Core.Dtos;

namespace LUMS.Core.Interfaces.Services;

public interface IDashboardService
{
    Task<AdminDashboardDto> GetAsync(CancellationToken ct = default);
}
