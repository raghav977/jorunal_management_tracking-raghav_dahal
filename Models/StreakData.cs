namespace MauiApp3.Models;

public class StreakData
{
    public int Id { get; set; }
    public int CurrentStreak { get; set; } = 0;
    public int LongestStreak { get; set; } = 0;
    public DateOnly? LastEntryDate { get; set; }
    public int TotalMissedDays { get; set; } = 0;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
