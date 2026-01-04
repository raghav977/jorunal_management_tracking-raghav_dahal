namespace MauiApp3.Services;

/// <summary>
/// Global application state for managing authentication and theme status across the app
/// </summary>
public class AppState
{
    public bool IsUnlocked { get; private set; } = false;
    public string CurrentTheme { get; private set; } = "light";

    public event Action? OnChange;

    public void SetUnlocked(bool unlocked)
    {
        IsUnlocked = unlocked;
        NotifyStateChanged();
    }

    public void Lock()
    {
        IsUnlocked = false;
        NotifyStateChanged();
    }

    public void Unlock()
    {
        IsUnlocked = true;
        NotifyStateChanged();
    }

    public void SetTheme(string theme)
    {
        CurrentTheme = theme.ToLower();
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
