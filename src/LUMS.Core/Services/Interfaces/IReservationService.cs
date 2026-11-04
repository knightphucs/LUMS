using LUMS.Core.Dtos;

namespace LUMS.Core.Services.Interfaces;

public interface IReservationService
{
    Task<IReadOnlyList<ReservationDto>> GetMyReservationsAsync(int readerId, CancellationToken ct = default);
}
