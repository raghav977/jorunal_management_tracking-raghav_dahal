namespace MauiApp3.Models;

public class JournalEntry
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    
    // Primary mood is required
    public MoodType Mood { get; set; } = MoodType.Neutral;
    
    // Up to two optional secondary moods
    public MoodType? SecondaryMood1 { get; set; }
    public MoodType? SecondaryMood2 { get; set; }
    
    // Date for one-entry-per-day constraint (stored as date only)
    public DateOnly EntryDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property for tags
    public List<JournalEntryTag> JournalEntryTags { get; set; } = new();
    
    // Computed word count
    public int WordCount => string.IsNullOrWhiteSpace(Content) 
        ? 0 
        : Content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
}
