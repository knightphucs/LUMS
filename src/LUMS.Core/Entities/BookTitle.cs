namespace LUMS.Core.Entities;

public class BookTitle
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImage { get; set; }
    public string DefaultShelf { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<Copy> Copies { get; set; } = new List<Copy>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
