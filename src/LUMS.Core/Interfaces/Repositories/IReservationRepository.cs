using LUMS.Core.Entities;

namespace LUMS.Core.Interfaces.Repositories;

public interface IReservationRepository : IRepository<Reservation>
{
    Task<IReadOnlyList<int>> GetBookIdsWithPendingReservationsAsync(
        IReadOnlyCollection<int> bookTitleIds, int excludeReaderId, CancellationToken ct = default);

    Task<IReadOnlyList<Reservation>> GetActiveByReaderAsync(int readerId, CancellationToken ct = default);
}
