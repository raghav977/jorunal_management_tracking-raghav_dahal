using Microsoft.EntityFrameworkCore;
using MauiApp3.Models;

namespace MauiApp3.Data;

public class AppDbContext : DbContext
{
    private static bool _initialized = false;
    private static readonly object _lock = new();
    
    public DbSet<UserSecurity> UserSecurities { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<JournalEntryTag> JournalEntryTags { get; set; }
    public DbSet<StreakData> StreakData { get; set; }
    public DbSet<ThemeSettings> ThemeSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "journal_app.db");
        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }

    public void InitializeDatabase()
    {
        if (_initialized) return;
        
        lock (_lock)
        {
            if (_initialized) return;
            
            try
            {
                // Try to ensure database exists with correct schema
                Database.EnsureCreated();
                
                // Verify tables exist by touching them
                _ = UserSecurities.Any();
                _ = Tags.Any();
                _ = JournalEntries.Any();
                _ = StreakData.Any();
                _ = ThemeSettings.Any();
            }
            catch
            {
                // Schema mismatch - delete and recreate
                Database.EnsureDeleted();
                Database.EnsureCreated();
            }
            
            _initialized = true;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // One entry per day constraint
        modelBuilder.Entity<JournalEntry>()
            .HasIndex(e => e.EntryDate)
            .IsUnique();

        // Many-to-many relationship between JournalEntry and Tag
        modelBuilder.Entity<JournalEntryTag>()
            .HasOne(jt => jt.JournalEntry)
            .WithMany(j => j.JournalEntryTags)
            .HasForeignKey(jt => jt.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JournalEntryTag>()
            .HasOne(jt => jt.Tag)
            .WithMany(t => t.JournalEntryTags)
            .HasForeignKey(jt => jt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed prebuilt tags
        modelBuilder.Entity<Tag>().HasData(
            new Tag { Id = 1, Name = "Personal", Color = "#444444", IsPrebuilt = true },
            new Tag { Id = 2, Name = "Work", Color = "#666666", IsPrebuilt = true },
            new Tag { Id = 3, Name = "Travel", Color = "#888888", IsPrebuilt = true },
            new Tag { Id = 4, Name = "Health", Color = "#333333", IsPrebuilt = true },
            new Tag { Id = 5, Name = "Goals", Color = "#555555", IsPrebuilt = true },
            new Tag { Id = 6, Name = "Reflection", Color = "#777777", IsPrebuilt = true }
        );
    }
}