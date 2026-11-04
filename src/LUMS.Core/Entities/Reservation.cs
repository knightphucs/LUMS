using LUMS.Core.Enums;

namespace LUMS.Core.Entities;

public class Reservation
{
    public int Id { get; set; }

    public int BookTitleId { get; set; }
    public BookTitle BookTitle { get; set; } = null!;

    public int ReaderId { get; set; }
    public Account Reader { get; set; } = null!;

    public DateTime ReservedAt { get; set; }

    public int QueuePosition { get; set; }

    public ReservationStatus Status { get; set; }
    public DateOnly? HoldUntil { get; set; }
}
