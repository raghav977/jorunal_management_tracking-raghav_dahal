namespace MauiApp3.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#666666"; // Default gray
    public bool IsPrebuilt { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public List<JournalEntryTag> JournalEntryTags { get; set; } = new();
}
