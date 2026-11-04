using LUMS.Core.Abstractions;

namespace LUMS.Tests.Fakes;

public sealed class FakeClock : IClock
{
    public FakeClock(DateTime now) => Now = now;

    public DateTime Now { get; set; }

    public DateOnly Today => DateOnly.FromDateTime(Now);
}
