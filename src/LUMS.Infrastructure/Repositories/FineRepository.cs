using LUMS.Core.Entities;
using LUMS.Core.Enums;
using LUMS.Core.Interfaces.Repositories;
using LUMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LUMS.Infrastructure.Repositories;

public class FineRepository : EfRepository<Fine>, IFineRepository
{
    public FineRepository(LumsDbContext db) : base(db)
    {
    }

    public async Task<decimal> SumCollectedAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var total = await Db.Fines.AsNoTracking()
            .Where(f => f.PaymentStatus == PaymentStatus.DaThanhToan && f.HandledAt >= from && f.HandledAt < to)
            .SumAsync(f => (decimal?)f.Amount, ct);
        return total ?? 0m;
    }
}
