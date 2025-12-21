using Microsoft.EntityFrameworkCore;
using MauiApp3.Models;

namespace MauiApp3.Data;

public class AppDbContext : DbContext
{
    public DbSet<UserSecurity> UserSecurities { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "appdb.db");
        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }
}