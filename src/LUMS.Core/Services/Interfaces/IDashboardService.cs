using LUMS.Core.Dtos;

namespace LUMS.Core.Services.Interfaces;

public interface IDashboardService
{
    Task<AdminDashboardDto> GetAsync(CancellationToken ct = default);
}
