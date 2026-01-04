using Microsoft.EntityFrameworkCore;
using MauiApp3.Data;
using MauiApp3.Models;

namespace MauiApp3.Services;

public class ThemeService : IThemeService
{
    private readonly AppDbContext _dbContext;

    public ThemeService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.Database.EnsureCreated();
    }

    public async Task<ThemeSettings> GetThemeAsync()
    {
        var theme = await _dbContext.ThemeSettings.FirstOrDefaultAsync();
        if (theme == null)
        {
            theme = new ThemeSettings
            {
                ThemeName = "Light",
                PrimaryColor = "#000000",
                BackgroundColor = "#FFFFFF",
                TextColor = "#000000",
                AccentColor = "#333333"
            };
            _dbContext.ThemeSettings.Add(theme);
            await _dbContext.SaveChangesAsync();
        }
        return theme;
    }

    public async Task SetThemeAsync(string themeName)
    {
        var theme = await GetThemeAsync();
        theme.ThemeName = themeName;

        switch (themeName)
        {
            case "Dark":
                theme.PrimaryColor = "#FFFFFF";
                theme.BackgroundColor = "#1a1a1a";
                theme.TextColor = "#FFFFFF";
                theme.AccentColor = "#cccccc";
                break;
            case "Light":
            default:
                theme.PrimaryColor = "#000000";
                theme.BackgroundColor = "#FFFFFF";
                theme.TextColor = "#000000";
                theme.AccentColor = "#333333";
                break;
        }

        theme.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task SetCustomThemeAsync(string primaryColor, string backgroundColor, string textColor, string accentColor)
    {
        var theme = await GetThemeAsync();
        theme.ThemeName = "Custom";
        theme.PrimaryColor = primaryColor;
        theme.BackgroundColor = backgroundColor;
        theme.TextColor = textColor;
        theme.AccentColor = accentColor;
        theme.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public List<string> GetAvailableThemes()
    {
        return new List<string> { "Light", "Dark", "Custom" };
    }
}
