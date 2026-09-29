using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskDTO>> GetAllAsync();
    Task<TaskDTO?> GetByIdAsync(int id);
    Task<IEnumerable<TaskDTO>> GetByProjectAsync(int projectId);
    Task<TaskDTO> CreateAsync(TaskCreateDTO dto);
    Task<TaskDTO?> UpdateAsync(int id, TaskUpdateDTO dto);
    Task<bool> SoftDeleteAsync(int id);
    Task<IEnumerable<TaskDTO>> SearchAsync(TaskSearchDTO criteria);
}
