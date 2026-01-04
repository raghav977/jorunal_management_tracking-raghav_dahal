using MauiApp3.Models;

namespace MauiApp3.Services;

public interface IJournalService
{
    // Basic CRUD
    Task<List<JournalEntry>> GetAllEntriesAsync();
    Task<JournalEntry?> GetEntryByIdAsync(int id);
    Task<JournalEntry> CreateEntryAsync(JournalEntry entry);
    Task<JournalEntry> UpdateEntryAsync(JournalEntry entry);
    Task DeleteEntryAsync(int id);
    
    // One entry per day
    Task<JournalEntry?> GetEntryByDateAsync(DateOnly date);
    Task<bool> HasEntryForDateAsync(DateOnly date);
    
    // Filtering
    Task<List<JournalEntry>> GetEntriesByMoodAsync(MoodType mood);
    Task<List<JournalEntry>> GetEntriesByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task<List<JournalEntry>> GetEntriesByTagAsync(int tagId);
    
    // Search
    Task<List<JournalEntry>> SearchEntriesAsync(string searchTerm);
    
    // Pagination
    Task<(List<JournalEntry> Entries, int TotalCount)> GetEntriesPagedAsync(int page, int pageSize, MoodType? moodFilter = null, int? tagId = null, string? searchTerm = null);
    
    // Analytics
    Task<Dictionary<MoodType, int>> GetMoodDistributionAsync();
    Task<int> GetTotalWordCountAsync();
    Task<List<(DateOnly Date, int WordCount)>> GetWordCountTrendAsync(int days = 30);
}
