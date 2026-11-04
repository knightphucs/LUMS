using LUMS.Core.Entities;

namespace LUMS.Core.Interfaces.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> GetByLoginAsync(string emailOrCode, CancellationToken ct = default);

    Task<int> CountAsync(CancellationToken ct = default);

    Task<IReadOnlyList<Account>> GetRecentAsync(int take, CancellationToken ct = default);
}
