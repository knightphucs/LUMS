using LUMS.Core.Entities;

namespace LUMS.Core.Repositories;

public interface IFineRepository : IRepository<Fine>
{
    Task<decimal> SumCollectedAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
