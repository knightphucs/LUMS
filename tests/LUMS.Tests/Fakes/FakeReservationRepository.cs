using LUMS.Core.Entities;
using LUMS.Core.Repositories;

namespace LUMS.Tests.Fakes;

public sealed class FakeReservationRepository : IReservationRepository
{
    public List<(int BookTitleId, int ReaderId)> Pending { get; } = new();

    public Task<IReadOnlyList<int>> GetBookIdsWithPendingReservationsAsync(
        IReadOnlyCollection<int> bookTitleIds, int excludeReaderId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<int>>(
            Pending.Where(p => p.ReaderId != excludeReaderId && bookTitleIds.Contains(p.BookTitleId))
                   .Select(p => p.BookTitleId)
                   .Distinct()
                   .ToList());

    public Task<IReadOnlyList<Reservation>> GetActiveByReaderAsync(int readerId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Reservation>>(Array.Empty<Reservation>());

    public Task<Reservation?> GetByIdAsync(object id, CancellationToken ct = default) => Task.FromResult<Reservation?>(null);

    public Task AddAsync(Reservation entity, CancellationToken ct = default) => Task.CompletedTask;

    public void Update(Reservation entity)
    {
    }

    public void Remove(Reservation entity)
    {
    }
}
