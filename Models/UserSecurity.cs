namespace MauiApp3.Models;

public class UserSecurity
{
    public int Id { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
