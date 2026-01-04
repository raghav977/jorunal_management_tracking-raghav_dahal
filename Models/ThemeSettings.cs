namespace MauiApp3.Models;

public class ThemeSettings
{
    public int Id { get; set; }
    public string ThemeName { get; set; } = "Light"; // Light, Dark, Custom
    public string PrimaryColor { get; set; } = "#000000";
    public string BackgroundColor { get; set; } = "#FFFFFF";
    public string TextColor { get; set; } = "#000000";
    public string AccentColor { get; set; } = "#333333";
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
