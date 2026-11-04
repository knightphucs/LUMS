using LUMS.Core.Entities;

namespace LUMS.Core.Repositories;

public interface ISystemLogRepository : IRepository<SystemLog>
{
    Task<IReadOnlyList<SystemLog>> GetRecentAsync(int take, CancellationToken ct = default);
}
