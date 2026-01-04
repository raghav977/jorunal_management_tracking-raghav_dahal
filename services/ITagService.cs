using MauiApp3.Models;

namespace MauiApp3.Services;

public interface ITagService
{
    Task<List<Tag>> GetAllTagsAsync();
    Task<List<Tag>> GetPrebuiltTagsAsync();
    Task<List<Tag>> GetCustomTagsAsync();
    Task<Tag?> GetTagByIdAsync(int id);
    Task<Tag> CreateTagAsync(Tag tag);
    Task<Tag> UpdateTagAsync(Tag tag);
    Task DeleteTagAsync(int id);
    Task<List<(Tag Tag, int Count)>> GetMostUsedTagsAsync(int top = 5);
    Task AddTagToEntryAsync(int entryId, int tagId);
    Task RemoveTagFromEntryAsync(int entryId, int tagId);
    Task SetEntryTagsAsync(int entryId, List<int> tagIds);
}
