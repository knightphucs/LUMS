using LUMS.Core.Entities;

namespace LUMS.Core.Repositories;

public interface IPolicyRepository : IRepository<PolicyConfiguration>
{
    Task<PolicyConfiguration?> GetCurrentAsync(CancellationToken ct = default);
}
