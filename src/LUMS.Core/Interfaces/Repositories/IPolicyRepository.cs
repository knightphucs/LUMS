using LUMS.Core.Entities;

namespace LUMS.Core.Interfaces.Repositories;

public interface IPolicyRepository : IRepository<PolicyConfiguration>
{
    Task<PolicyConfiguration?> GetCurrentAsync(CancellationToken ct = default);
}
