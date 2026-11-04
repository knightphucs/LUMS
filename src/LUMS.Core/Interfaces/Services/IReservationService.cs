using LUMS.Core.Dtos;

namespace LUMS.Core.Interfaces.Services;

public interface IReservationService
{
    Task<IReadOnlyList<ReservationDto>> GetMyReservationsAsync(int readerId, CancellationToken ct = default);
}
