using MauiApp3.Models;

namespace MauiApp3.Services;

public interface IThemeService
{
    Task<ThemeSettings> GetThemeAsync();
    Task SetThemeAsync(string themeName);
    Task SetCustomThemeAsync(string primaryColor, string backgroundColor, string textColor, string accentColor);
    List<string> GetAvailableThemes();
}
