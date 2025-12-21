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

    public async Task<List<JournalEntry>> GetAllEntriesAsync()
    {
        return await _dbContext.JournalEntries
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<JournalEntry?> GetEntryByIdAsync(int id)
    {
        return await _dbContext.JournalEntries.FindAsync(id);
    }

    public async Task<JournalEntry> CreateEntryAsync(JournalEntry entry)
    {
        entry.CreatedAt = DateTime.UtcNow;
        entry.UpdatedAt = DateTime.UtcNow;
        _dbContext.JournalEntries.Add(entry);
        await _dbContext.SaveChangesAsync();
        return entry;
    }

    public async Task<JournalEntry> UpdateEntryAsync(JournalEntry entry)
    {
        var existing = await _dbContext.JournalEntries.FindAsync(entry.Id);
        if (existing == null)
        {
            throw new InvalidOperationException($"Journal entry with ID {entry.Id} not found.");
        }

        existing.Title = entry.Title;
        existing.Content = entry.Content;
        existing.Mood = entry.Mood;
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

    public async Task<List<JournalEntry>> GetEntriesByMoodAsync(MoodType mood)
    {
        return await _dbContext.JournalEntries
            .Where(e => e.Mood == mood)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }
}
