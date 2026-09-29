using TaskTrack.Repo.Entities;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _uow;

    public DepartmentService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<DepartmentDTO>> GetAllAsync()
    {
        var departments = await _uow.Departments.GetAllAsync();
        return departments.Where(d => d.IsActive).Select(ToDTO);
    }

    public async Task<DepartmentDetailDTO?> GetByIdAsync(int id)
    {
        var department = await _uow.Departments.GetByIdAsync(id);
        if (department == null) return null;

        var projectDtos = (await _uow.Projects.GetAllAsync())
            .Where(p => p.DepartmentID == id && p.IsActive)
            .Select(p => new ProjectDTO
            {
                ProjectID = p.ProjectID,
                ProjectName = p.ProjectName,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                DepartmentID = p.DepartmentID,
                IsActive = p.IsActive,
                CreatedDate = p.CreatedDate
            })
            .ToList();

        return new DepartmentDetailDTO
        {
            DepartmentID = department.DepartmentID,
            DepartmentName = department.DepartmentName,
            DepartmentDescription = department.DepartmentDescription,
            IsActive = department.IsActive,
            Projects = projectDtos
        };
    }

    public async Task<IEnumerable<DepartmentDTO>> SearchByNameAsync(string name)
    {
        var q = name?.Trim();
        if (string.IsNullOrWhiteSpace(q))
            return await GetAllAsync();

        var departments = await _uow.Departments.GetAllAsync();
        return departments
            .Where(d => d.IsActive && d.DepartmentName.Contains(q, StringComparison.OrdinalIgnoreCase))
            .Select(ToDTO);
    }

    public async Task<DepartmentDTO> CreateAsync(DepartmentCreateDTO dto)
    {
        var entity = new Department
        {
            DepartmentName = dto.DepartmentName.Trim(),
            DepartmentDescription = dto.DepartmentDescription ?? string.Empty,
            IsActive = dto.IsActive
        };

        await _uow.Departments.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return ToDTO(entity);
    }

    public async Task<DepartmentDTO?> UpdateAsync(int id, DepartmentUpdateDTO dto)
    {
        var department = await _uow.Departments.GetByIdAsync(id);
        if (department == null) return null;

        department.DepartmentName = dto.DepartmentName.Trim();
        department.DepartmentDescription = dto.DepartmentDescription ?? string.Empty;
        department.IsActive = dto.IsActive;

        _uow.Departments.Update(department);
        await _uow.SaveChangesAsync();
        return ToDTO(department);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var department = await _uow.Departments.GetByIdAsync(id);
        if (department == null) return false;

        var hasProjects = (await _uow.Projects.GetAllAsync()).Any(p => p.DepartmentID == id);
        if (hasProjects) return false;

        department.IsActive = false;
        _uow.Departments.Update(department);
        await _uow.SaveChangesAsync();
        return true;
    }

    private static DepartmentDTO ToDTO(Department department) => new()
    {
        DepartmentID = department.DepartmentID,
        DepartmentName = department.DepartmentName,
        DepartmentDescription = department.DepartmentDescription,
        IsActive = department.IsActive
    };
}
