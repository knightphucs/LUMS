using LUMS.Core.Dtos;
using LUMS.Core.Repositories;
using LUMS.Core.Services.Interfaces;

namespace LUMS.Core.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservations;

    public ReservationService(IReservationRepository reservations)
    {
        _reservations = reservations;
    }

    public async Task<IReadOnlyList<ReservationDto>> GetMyReservationsAsync(int readerId, CancellationToken ct = default)
    {
        var items = await _reservations.GetActiveByReaderAsync(readerId, ct);
        return items
            .Select(r => new ReservationDto(r.Id, r.BookTitle.Title, r.Status, r.QueuePosition, r.HoldUntil))
            .ToList();
    }
}
