using Microsoft.EntityFrameworkCore;
using MauiApp3.Data;
using MauiApp3.Models;

namespace MauiApp3.Services;

public class StreakService : IStreakService
{
    private readonly AppDbContext _dbContext;

    public StreakService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.Database.EnsureCreated();
    }

    public async Task<StreakData> GetStreakDataAsync()
    {
        var streakData = await _dbContext.StreakData.FirstOrDefaultAsync();
        if (streakData == null)
        {
            streakData = new StreakData();
            _dbContext.StreakData.Add(streakData);
            await _dbContext.SaveChangesAsync();
        }
        return streakData;
    }

    public async Task UpdateStreakAsync()
    {
        var streakData = await GetStreakDataAsync();
        var today = DateOnly.FromDateTime(DateTime.Now);

        // Get all entries ordered by date
        var entries = await _dbContext.JournalEntries
            .OrderByDescending(e => e.EntryDate)
            .Select(e => e.EntryDate)
            .ToListAsync();

        if (!entries.Any())
        {
            streakData.CurrentStreak = 0;
            streakData.LastEntryDate = null;
            streakData.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return;
        }

        var lastEntryDate = entries.First();
        streakData.LastEntryDate = lastEntryDate;

        // Calculate current streak
        var currentStreak = 0;
        var checkDate = today;

        // If no entry today, check from yesterday
        if (!entries.Contains(today))
        {
            checkDate = today.AddDays(-1);
        }

        foreach (var date in entries.OrderByDescending(d => d))
        {
            if (date == checkDate)
            {
                currentStreak++;
                checkDate = checkDate.AddDays(-1);
            }
            else if (date < checkDate)
            {
                break;
            }
        }

        streakData.CurrentStreak = currentStreak;

        // Update longest streak if current is higher
        if (currentStreak > streakData.LongestStreak)
        {
            streakData.LongestStreak = currentStreak;
        }

        // Calculate missed days (days without entries since first entry)
        if (entries.Any())
        {
            var firstEntry = entries.Last();
            var totalDays = today.DayNumber - firstEntry.DayNumber + 1;
            streakData.TotalMissedDays = totalDays - entries.Distinct().Count();
        }

        streakData.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<int> GetCurrentStreakAsync()
    {
        await UpdateStreakAsync();
        var data = await GetStreakDataAsync();
        return data.CurrentStreak;
    }

    public async Task<int> GetLongestStreakAsync()
    {
        var data = await GetStreakDataAsync();
        return data.LongestStreak;
    }

    public async Task<int> GetMissedDaysAsync()
    {
        var data = await GetStreakDataAsync();
        return data.TotalMissedDays;
    }
}
