using TaskTrack.Repo.Entities;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _uow;

    public ProjectService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<ProjectDTO>> GetAllAsync()
    {
        var projects = await _uow.Projects.GetAllAsync();
        var depts = (await _uow.Departments.GetAllAsync()).ToDictionary(d => d.DepartmentID, d => d.DepartmentName);
        return projects.Where(p => p.IsActive)
                       .Select(p => ToDTO(p, depts.GetValueOrDefault(p.DepartmentID)));
    }

    public async Task<ProjectDetailDTO?> GetByIdAsync(int id)
    {
        var p = await _uow.Projects.GetByIdAsync(id);
        if (p == null) return null;

        var taskItems = (await _uow.Tasks.GetAllAsync())
            .Where(t => t.ProjectID == id && t.IsActive)
            .ToList();
        var taskIds = taskItems.Select(task => task.TaskID).ToHashSet();
        var links = taskIds.Count == 0
            ? new List<TaskTag>()
            : (await _uow.TaskTags.FindAsync(link => taskIds.Contains(link.TaskID))).ToList();
        var tagIds = links.Select(link => link.TagID).Distinct().ToHashSet();
        var tags = (await _uow.Tags.GetAllAsync())
            .Where(tag => tagIds.Contains(tag.TagID))
            .ToDictionary(tag => tag.TagID);
        var tasks = taskItems.Select(task => new TaskDTO
            {
                TaskID = task.TaskID, Title = task.Title, Description = task.Description,
                Status = task.Status, Priority = task.Priority, DueDate = task.DueDate,
                ProjectID = task.ProjectID, ProjectName = p.ProjectName, IsActive = task.IsActive,
                CreatedDate = task.CreatedDate, ModifiedDate = task.ModifiedDate,
                Tags = links.Where(link => link.TaskID == task.TaskID && tags.ContainsKey(link.TagID))
                    .Select(link => tags[link.TagID])
                    .Select(tag => new TagDTO { TagID = tag.TagID, TagName = tag.TagName, Color = tag.Color })
                    .ToList()
            }).ToList();
        var department = await _uow.Departments.GetByIdAsync(p.DepartmentID);

        var dto = new ProjectDetailDTO
        {
            ProjectID = p.ProjectID, ProjectName = p.ProjectName, Description = p.Description,
            StartDate = p.StartDate, EndDate = p.EndDate, Status = p.Status,
            DepartmentID = p.DepartmentID, DepartmentName = department?.DepartmentName,
            IsActive = p.IsActive, CreatedDate = p.CreatedDate,
            Tasks = tasks
        };
        return dto;
    }

    public async Task<IEnumerable<ProjectDTO>> GetByDepartmentAsync(int departmentId)
    {
        var projects = await _uow.Projects.GetAllAsync();
        var department = await _uow.Departments.GetByIdAsync(departmentId);
        return projects.Where(p => p.DepartmentID == departmentId && p.IsActive)
            .Select(project => ToDTO(project, department?.DepartmentName));
    }

    public async Task<ProjectDTO> CreateAsync(ProjectCreateDTO dto)
    {
        var p = new Project
        {
            ProjectName = dto.ProjectName.Trim(),
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = (short)dto.Status,
            DepartmentID = dto.DepartmentID,
            IsActive = dto.IsActive,
            CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };
        await _uow.Projects.AddAsync(p);
        await _uow.SaveChangesAsync();
        return ToDTOSimple(p);
    }

    public async Task<ProjectDTO?> UpdateAsync(int id, ProjectUpdateDTO dto)
    {
        var p = await _uow.Projects.GetByIdAsync(id);
        if (p == null) return null;

        p.ProjectName = dto.ProjectName.Trim();
        p.Description = dto.Description;
        p.StartDate = dto.StartDate;
        p.EndDate = dto.EndDate;
        p.Status = (short)dto.Status;
        p.DepartmentID = dto.DepartmentID;
        p.IsActive = dto.IsActive;

        _uow.Projects.Update(p);
        await _uow.SaveChangesAsync();
        return ToDTOSimple(p);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var p = await _uow.Projects.GetByIdAsync(id);
        if (p == null) return false;

        var hasTasks = (await _uow.Tasks.GetAllAsync()).Any(t => t.ProjectID == id);
        if (hasTasks) return false; // HTTP 400

        _uow.Projects.Remove(p);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ProjectDTO>> SearchAsync(ProjectSearchDTO c)
    {
        var projects = await _uow.Projects.GetAllAsync();
        var query = projects.Where(p => p.IsActive).AsQueryable();

        if (!string.IsNullOrWhiteSpace(c.Name))
            query = query.Where(p => p.ProjectName.ToLower().Contains(c.Name.ToLower().Trim()));
        if (c.Status.HasValue)
            query = query.Where(p => p.Status == c.Status.Value);
        if (c.DepartmentId.HasValue)
            query = query.Where(p => p.DepartmentID == c.DepartmentId.Value);

        return query.Select(ToDTOSimple);
    }

    private static ProjectDTO ToDTO(Project p, string? deptName) => new()
    {
        ProjectID = p.ProjectID, ProjectName = p.ProjectName, Description = p.Description,
        StartDate = p.StartDate, EndDate = p.EndDate, Status = p.Status,
        DepartmentID = p.DepartmentID, DepartmentName = deptName,
        IsActive = p.IsActive, CreatedDate = p.CreatedDate
    };

    private static ProjectDTO ToDTOSimple(Project p) => ToDTO(p, p.Department?.DepartmentName);
}
