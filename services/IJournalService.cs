using MauiApp3.Models;

namespace MauiApp3.Services;

public interface IJournalService
{
    Task<List<JournalEntry>> GetAllEntriesAsync();
    Task<JournalEntry?> GetEntryByIdAsync(int id);
    Task<JournalEntry> CreateEntryAsync(JournalEntry entry);
    Task<JournalEntry> UpdateEntryAsync(JournalEntry entry);
    Task DeleteEntryAsync(int id);
    Task<List<JournalEntry>> GetEntriesByMoodAsync(MoodType mood);
}
