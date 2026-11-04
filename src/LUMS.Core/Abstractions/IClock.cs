namespace LUMS.Core.Abstractions;

public interface IClock
{
    DateTime Now { get; }
    DateOnly Today { get; }
}
