using LUMS.Core.Abstractions;
using LUMS.Infrastructure.Data;

namespace LUMS.Infrastructure.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly LumsDbContext _db;

    public UnitOfWork(LumsDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
