using Microsoft.EntityFrameworkCore;
using MauiApp3.Data;
using MauiApp3.Models;

namespace MauiApp3.Services;

public class JournalService : IJournalService
{
    private readonly AppDbContext _dbContext;

    public JournalService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.Database.EnsureCreated();
    }

    // Basic CRUD
    public async Task<List<JournalEntry>> GetAllEntriesAsync()
    {
        return await _dbContext.JournalEntries
            .Include(e => e.JournalEntryTags)
            .ThenInclude(jt => jt.Tag)
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
    }

    public async Task<JournalEntry?> GetEntryByIdAsync(int id)
    {
        return await _dbContext.JournalEntries
            .Include(e => e.JournalEntryTags)
            .ThenInclude(jt => jt.Tag)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<JournalEntry> CreateEntryAsync(JournalEntry entry)
    {
        // One entry per day - check if entry exists for this date
        var existingEntry = await GetEntryByDateAsync(entry.EntryDate);
        if (existingEntry != null)
        {
            throw new InvalidOperationException($"An entry already exists for {entry.EntryDate:MMM dd, yyyy}. Only one entry per day is allowed.");
        }

        entry.CreatedAt = DateTime.UtcNow;
        entry.UpdatedAt = DateTime.UtcNow;
        _dbContext.JournalEntries.Add(entry);
        await _dbContext.SaveChangesAsync();
        return entry;
    }

    public async Task<JournalEntry> UpdateEntryAsync(JournalEntry entry)
    {
        var existing = await _dbContext.JournalEntries
            .Include(e => e.JournalEntryTags)
            .FirstOrDefaultAsync(e => e.Id == entry.Id);
            
        if (existing == null)
        {
            throw new InvalidOperationException($"Journal entry with ID {entry.Id} not found.");
        }

        existing.Title = entry.Title;
        existing.Content = entry.Content;
        existing.Mood = entry.Mood;
        existing.SecondaryMood1 = entry.SecondaryMood1;
        existing.SecondaryMood2 = entry.SecondaryMood2;
        existing.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteEntryAsync(int id)
    {
        var entry = await _dbContext.JournalEntries.FindAsync(id);
        if (entry != null)
        {
            _dbContext.JournalEntries.Remove(entry);
            await _dbContext.SaveChangesAsync();
        }
    }

    // One entry per day
    public async Task<JournalEntry?> GetEntryByDateAsync(DateOnly date)
    {
        return await _dbContext.JournalEntries
            .Include(e => e.JournalEntryTags)
            .ThenInclude(jt => jt.Tag)
            .FirstOrDefaultAsync(e => e.EntryDate == date);
    }

    public async Task<bool> HasEntryForDateAsync(DateOnly date)
    {
        return await _dbContext.JournalEntries.AnyAsync(e => e.EntryDate == date);
    }

    // Filtering
    public async Task<List<JournalEntry>> GetEntriesByMoodAsync(MoodType mood)
    {
        return await _dbContext.JournalEntries
            .Include(e => e.JournalEntryTags)
            .ThenInclude(jt => jt.Tag)
            .Where(e => e.Mood == mood || e.SecondaryMood1 == mood || e.SecondaryMood2 == mood)
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
    }

    public async Task<List<JournalEntry>> GetEntriesByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await _dbContext.JournalEntries
            .Include(e => e.JournalEntryTags)
            .ThenInclude(jt => jt.Tag)
            .Where(e => e.EntryDate >= startDate && e.EntryDate <= endDate)
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
    }

    public async Task<List<JournalEntry>> GetEntriesByTagAsync(int tagId)
    {
        return await _dbContext.JournalEntries
            .Include(e => e.JournalEntryTags)
            .ThenInclude(jt => jt.Tag)
            .Where(e => e.JournalEntryTags.Any(jt => jt.TagId == tagId))
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
    }

    // Search
    public async Task<List<JournalEntry>> SearchEntriesAsync(string searchTerm)
    {
        var lowerTerm = searchTerm.ToLower();
        return await _dbContext.JournalEntries
            .Include(e => e.JournalEntryTags)
            .ThenInclude(jt => jt.Tag)
            .Where(e => e.Title.ToLower().Contains(lowerTerm) || e.Content.ToLower().Contains(lowerTerm))
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
    }

    // Pagination
    public async Task<(List<JournalEntry> Entries, int TotalCount)> GetEntriesPagedAsync(
        int page, int pageSize, MoodType? moodFilter = null, int? tagId = null, string? searchTerm = null)
    {
        var query = _dbContext.JournalEntries
            .Include(e => e.JournalEntryTags)
            .ThenInclude(jt => jt.Tag)
            .AsQueryable();

        // Apply filters
        if (moodFilter.HasValue)
        {
            query = query.Where(e => e.Mood == moodFilter.Value || 
                                      e.SecondaryMood1 == moodFilter.Value || 
                                      e.SecondaryMood2 == moodFilter.Value);
        }

        if (tagId.HasValue)
        {
            query = query.Where(e => e.JournalEntryTags.Any(jt => jt.TagId == tagId.Value));
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerTerm = searchTerm.ToLower();
            query = query.Where(e => e.Title.ToLower().Contains(lowerTerm) || 
                                      e.Content.ToLower().Contains(lowerTerm));
        }

        var totalCount = await query.CountAsync();
        var entries = await query
            .OrderByDescending(e => e.EntryDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (entries, totalCount);
    }

    // Analytics
    public async Task<Dictionary<MoodType, int>> GetMoodDistributionAsync()
    {
        var entries = await _dbContext.JournalEntries.ToListAsync();
        var distribution = new Dictionary<MoodType, int>();

        foreach (MoodType mood in Enum.GetValues<MoodType>())
        {
            distribution[mood] = 0;
        }

        foreach (var entry in entries)
        {
            distribution[entry.Mood]++;
            if (entry.SecondaryMood1.HasValue)
                distribution[entry.SecondaryMood1.Value]++;
            if (entry.SecondaryMood2.HasValue)
                distribution[entry.SecondaryMood2.Value]++;
        }

        return distribution;
    }

    public async Task<int> GetTotalWordCountAsync()
    {
        var entries = await _dbContext.JournalEntries.ToListAsync();
        return entries.Sum(e => e.WordCount);
    }

    public async Task<List<(DateOnly Date, int WordCount)>> GetWordCountTrendAsync(int days = 30)
    {
        var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-days));
        var entries = await _dbContext.JournalEntries
            .Where(e => e.EntryDate >= startDate)
            .OrderBy(e => e.EntryDate)
            .ToListAsync();

        return entries.Select(e => (e.EntryDate, e.WordCount)).ToList();
    }
}
