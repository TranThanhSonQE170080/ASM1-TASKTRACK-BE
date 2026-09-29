using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public interface ITagService
{
    Task<IEnumerable<TagDTO>> GetAllAsync();
    Task<TagDTO> CreateAsync(TagCreateDTO dto);
    Task<TagDTO?> UpdateAsync(int id, TagUpdateDTO dto);
    Task<bool> DeleteAsync(int id); // false => used by tasks (HTTP 400)
}
