using TaskTrack.Repo.Entities;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public class TagService : ITagService
{
    private readonly IUnitOfWork _uow;

    public TagService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<TagDTO>> GetAllAsync()
    {
        var tags = await _uow.Tags.GetAllAsync();
        return tags.Select(t => new TagDTO { TagID = t.TagID, TagName = t.TagName, Color = t.Color });
    }

    public async Task<TagDTO> CreateAsync(TagCreateDTO dto)
    {
        var tag = new Tag { TagName = dto.TagName.Trim(), Color = dto.Color };
        await _uow.Tags.AddAsync(tag);
        await _uow.SaveChangesAsync();
        return new TagDTO { TagID = tag.TagID, TagName = tag.TagName, Color = tag.Color };
    }

    public async Task<TagDTO?> UpdateAsync(int id, TagUpdateDTO dto)
    {
        var tag = await _uow.Tags.GetByIdAsync(id);
        if (tag == null) return null;

        tag.TagName = dto.TagName.Trim();
        tag.Color = dto.Color;
        _uow.Tags.Update(tag);
        await _uow.SaveChangesAsync();
        return new TagDTO { TagID = tag.TagID, TagName = tag.TagName, Color = tag.Color };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tag = await _uow.Tags.GetByIdAsync(id);
        if (tag == null) return false;

        var used = (await _uow.TaskTags.GetAllAsync()).Any(tt => tt.TagID == id);
        if (used) return false; // HTTP 400

        _uow.Tags.Remove(tag);
        await _uow.SaveChangesAsync();
        return true;
    }
}
