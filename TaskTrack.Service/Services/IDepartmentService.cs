using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDTO>> GetAllAsync();
    Task<DepartmentDetailDTO?> GetByIdAsync(int id);
    Task<IEnumerable<DepartmentDTO>> SearchByNameAsync(string name);
    Task<DepartmentDTO> CreateAsync(DepartmentCreateDTO dto);
    Task<DepartmentDTO?> UpdateAsync(int id, DepartmentUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
}
