using MauiApp3.Models;

namespace MauiApp3.Services;

public interface IStreakService
{
    Task<StreakData> GetStreakDataAsync();
    Task UpdateStreakAsync();
    Task<int> GetCurrentStreakAsync();
    Task<int> GetLongestStreakAsync();
    Task<int> GetMissedDaysAsync();
}
