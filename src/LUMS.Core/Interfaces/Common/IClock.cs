namespace LUMS.Core.Interfaces.Common;

public interface IClock
{
    DateTime Now { get; }
    DateOnly Today { get; }
}
