using LUMS.Core.Entities;

namespace LUMS.Core.Interfaces.Repositories;

public interface IFineRepository : IRepository<Fine>
{
    Task<decimal> SumCollectedAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
