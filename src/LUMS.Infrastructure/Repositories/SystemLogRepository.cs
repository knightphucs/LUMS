using LUMS.Core.Entities;
using LUMS.Core.Interfaces.Repositories;
using LUMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LUMS.Infrastructure.Repositories;

public class SystemLogRepository : EfRepository<SystemLog>, ISystemLogRepository
{
    public SystemLogRepository(LumsDbContext db) : base(db)
    {
    }

    public async Task<IReadOnlyList<SystemLog>> GetRecentAsync(int take, CancellationToken ct = default) =>
        await Db.SystemLogs.AsNoTracking()
            .Include(l => l.Account)
            .OrderByDescending(l => l.Timestamp).ThenByDescending(l => l.Id)
            .Take(take)
            .ToListAsync(ct);
}
