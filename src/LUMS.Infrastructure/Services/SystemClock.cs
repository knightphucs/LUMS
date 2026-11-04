using LUMS.Core.Interfaces.Common;

namespace LUMS.Infrastructure.Services;

public class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;

    public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
}
