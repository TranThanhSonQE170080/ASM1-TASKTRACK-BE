using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectDTO>> GetAllAsync();
    Task<ProjectDetailDTO?> GetByIdAsync(int id);
    Task<IEnumerable<ProjectDTO>> GetByDepartmentAsync(int departmentId);
    Task<ProjectDTO> CreateAsync(ProjectCreateDTO dto);
    Task<ProjectDTO?> UpdateAsync(int id, ProjectUpdateDTO dto);
    Task<bool> DeleteAsync(int id); // false => has tasks (HTTP 400)
    Task<IEnumerable<ProjectDTO>> SearchAsync(ProjectSearchDTO criteria);
}
