using LUMS.Core.Entities;
using LUMS.Core.Enums;
using LUMS.Core.Interfaces.Repositories;
using LUMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LUMS.Infrastructure.Repositories;

public class ReservationRepository : EfRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(LumsDbContext db) : base(db)
    {
    }

    public async Task<IReadOnlyList<int>> GetBookIdsWithPendingReservationsAsync(
        IReadOnlyCollection<int> bookTitleIds, int excludeReaderId, CancellationToken ct = default)
    {
        var ids = bookTitleIds.ToList();
        if (ids.Count == 0)
        {
            return Array.Empty<int>();
        }

        return await Db.Reservations.AsNoTracking()
            .Where(r => ids.Contains(r.BookTitleId)
                        && r.ReaderId != excludeReaderId
                        && (r.Status == ReservationStatus.DangCho || r.Status == ReservationStatus.SanSangNhanSach))
            .Select(r => r.BookTitleId)
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Reservation>> GetActiveByReaderAsync(int readerId, CancellationToken ct = default) =>
        await Db.Reservations.AsNoTracking()
            .Include(r => r.BookTitle)
            .Where(r => r.ReaderId == readerId
                        && (r.Status == ReservationStatus.DangCho || r.Status == ReservationStatus.SanSangNhanSach))
            .OrderBy(r => r.ReservedAt)
            .ToListAsync(ct);
}
