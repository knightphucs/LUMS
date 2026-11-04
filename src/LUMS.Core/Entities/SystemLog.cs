namespace LUMS.Core.Entities;

public class SystemLog
{
    public int Id { get; set; }

    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public string Action { get; set; } = string.Empty;

    public string? RelatedObject { get; set; }

    public DateTime Timestamp { get; set; }
}
