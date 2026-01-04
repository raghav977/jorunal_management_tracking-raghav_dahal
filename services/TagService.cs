using Microsoft.EntityFrameworkCore;
using MauiApp3.Data;
using MauiApp3.Models;

namespace MauiApp3.Services;

public class TagService : ITagService
{
    private readonly AppDbContext _dbContext;

    public TagService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.Database.EnsureCreated();
    }

    public async Task<List<Tag>> GetAllTagsAsync()
    {
        return await _dbContext.Tags
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<List<Tag>> GetPrebuiltTagsAsync()
    {
        return await _dbContext.Tags
            .Where(t => t.IsPrebuilt)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<List<Tag>> GetCustomTagsAsync()
    {
        return await _dbContext.Tags
            .Where(t => !t.IsPrebuilt)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<Tag?> GetTagByIdAsync(int id)
    {
        return await _dbContext.Tags.FindAsync(id);
    }

    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        tag.IsPrebuilt = false;
        tag.CreatedAt = DateTime.UtcNow;
        _dbContext.Tags.Add(tag);
        await _dbContext.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag> UpdateTagAsync(Tag tag)
    {
        var existing = await _dbContext.Tags.FindAsync(tag.Id);
        if (existing == null)
        {
            throw new InvalidOperationException($"Tag with ID {tag.Id} not found.");
        }

        existing.Name = tag.Name;
        existing.Color = tag.Color;
        await _dbContext.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteTagAsync(int id)
    {
        var tag = await _dbContext.Tags.FindAsync(id);
        if (tag != null && !tag.IsPrebuilt)
        {
            _dbContext.Tags.Remove(tag);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<(Tag Tag, int Count)>> GetMostUsedTagsAsync(int top = 5)
    {
        var tagCounts = await _dbContext.Tags
            .Include(t => t.JournalEntryTags)
            .Select(t => new { Tag = t, Count = t.JournalEntryTags.Count })
            .OrderByDescending(x => x.Count)
            .Take(top)
            .ToListAsync();

        return tagCounts.Select(x => (x.Tag, x.Count)).ToList();
    }

    public async Task AddTagToEntryAsync(int entryId, int tagId)
    {
        var exists = await _dbContext.JournalEntryTags
            .AnyAsync(jt => jt.JournalEntryId == entryId && jt.TagId == tagId);

        if (!exists)
        {
            _dbContext.JournalEntryTags.Add(new JournalEntryTag
            {
                JournalEntryId = entryId,
                TagId = tagId
            });
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RemoveTagFromEntryAsync(int entryId, int tagId)
    {
        var jt = await _dbContext.JournalEntryTags
            .FirstOrDefaultAsync(jt => jt.JournalEntryId == entryId && jt.TagId == tagId);

        if (jt != null)
        {
            _dbContext.JournalEntryTags.Remove(jt);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task SetEntryTagsAsync(int entryId, List<int> tagIds)
    {
        // Remove existing tags
        var existingTags = await _dbContext.JournalEntryTags
            .Where(jt => jt.JournalEntryId == entryId)
            .ToListAsync();
        _dbContext.JournalEntryTags.RemoveRange(existingTags);

        // Add new tags
        foreach (var tagId in tagIds)
        {
            _dbContext.JournalEntryTags.Add(new JournalEntryTag
            {
                JournalEntryId = entryId,
                TagId = tagId
            });
        }

        await _dbContext.SaveChangesAsync();
    }
}
