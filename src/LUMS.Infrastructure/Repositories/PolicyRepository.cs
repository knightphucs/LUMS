using LUMS.Core.Entities;
using LUMS.Core.Repositories;
using LUMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LUMS.Infrastructure.Repositories;

public class PolicyRepository : EfRepository<PolicyConfiguration>, IPolicyRepository
{
    public PolicyRepository(LumsDbContext db) : base(db)
    {
    }

    public Task<PolicyConfiguration?> GetCurrentAsync(CancellationToken ct = default) =>
        Db.PolicyConfigurations.AsNoTracking()
            .OrderByDescending(p => p.EffectiveFrom).ThenByDescending(p => p.Id)
            .FirstOrDefaultAsync(ct);
}
