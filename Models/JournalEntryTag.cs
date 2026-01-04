namespace MauiApp3.Models;

// Junction table for many-to-many relationship between JournalEntry and Tag
public class JournalEntryTag
{
    public int Id { get; set; }
    public int JournalEntryId { get; set; }
    public int TagId { get; set; }
    
    // Navigation properties
    public JournalEntry JournalEntry { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
