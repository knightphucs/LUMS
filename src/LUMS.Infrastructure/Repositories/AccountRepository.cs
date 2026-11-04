using LUMS.Core.Entities;
using LUMS.Core.Interfaces.Repositories;
using LUMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LUMS.Infrastructure.Repositories;

public class AccountRepository : EfRepository<Account>, IAccountRepository
{
    public AccountRepository(LumsDbContext db) : base(db)
    {
    }

    public Task<Account?> GetByLoginAsync(string emailOrCode, CancellationToken ct = default) =>
        Db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Email == emailOrCode || a.Code == emailOrCode, ct);

    public Task<int> CountAsync(CancellationToken ct = default) => Db.Accounts.CountAsync(ct);

    public async Task<IReadOnlyList<Account>> GetRecentAsync(int take, CancellationToken ct = default) =>
        await Db.Accounts.AsNoTracking()
            .OrderByDescending(a => a.CreatedAt).ThenByDescending(a => a.Id)
            .Take(take)
            .ToListAsync(ct);
}
