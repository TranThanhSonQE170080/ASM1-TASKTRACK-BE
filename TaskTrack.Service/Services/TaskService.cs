using TaskTrack.Repo.Entities;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _uow;

    public TaskService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<TaskDTO>> GetAllAsync()
    {
        var tasks = await _uow.Tasks.GetAllAsync();
        return await AddProjectNamesAsync(tasks.Where(t => t.IsActive));
    }

    public async Task<TaskDTO?> GetByIdAsync(int id)
    {
        var t = await _uow.Tasks.GetByIdAsync(id);
        if (t == null) return null;

        var dto = ToDTOSimple(t);
        var project = await _uow.Projects.GetByIdAsync(t.ProjectID);
        dto.ProjectName = project?.ProjectName;
        dto.Tags = await GetTaskTagsAsync(id);
        return dto;
    }

    public async Task<IEnumerable<TaskDTO>> GetByProjectAsync(int projectId)
    {
        var tasks = await _uow.Tasks.GetAllAsync();
        return await AddProjectNamesAsync(tasks.Where(t => t.ProjectID == projectId && t.IsActive));
    }

    public async Task<TaskDTO> CreateAsync(TaskCreateDTO dto)
    {
        var t = new TaskItem
        {
            Title = dto.Title.Trim(),
            Description = dto.Description,
            Status = (short)dto.Status,
            Priority = (short)dto.Priority,
            DueDate = dto.DueDate,
            ProjectID = dto.ProjectID,
            IsActive = true,
            CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        if (dto.TagIDs is { Count: > 0 })
        {
            foreach (var tagId in dto.TagIDs.Distinct())
                t.TaskTags.Add(new TaskTag { TagID = tagId, Task = t });
        }

        await _uow.Tasks.AddAsync(t);
        await _uow.SaveChangesAsync();

        var result = ToDTOSimple(t);
        result.Tags = await GetTagDTOsAsync(dto.TagIDs ?? new List<int>());
        var project = await _uow.Projects.GetByIdAsync(t.ProjectID);
        result.ProjectName = project?.ProjectName;
        return result;
    }

    public async Task<TaskDTO?> UpdateAsync(int id, TaskUpdateDTO dto)
    {
        var t = await _uow.Tasks.GetByIdAsync(id);
        if (t == null) return null;

        t.Title = dto.Title.Trim();
        t.Description = dto.Description;
        t.Status = (short)dto.Status;
        t.Priority = (short)dto.Priority;
        t.DueDate = dto.DueDate;
        t.ProjectID = dto.ProjectID;
        t.ModifiedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        // replace tags
        var taskTagRepo = _uow.TaskTags;
        var oldLinks = await taskTagRepo.FindAsync(tt => tt.TaskID == id);
        foreach (var link in oldLinks)
            taskTagRepo.Remove(link);

        if (dto.TagIDs is { Count: > 0 })
        {
            foreach (var tagId in dto.TagIDs.Distinct())
                await taskTagRepo.AddAsync(new TaskTag { TaskID = id, TagID = tagId });
        }

        _uow.Tasks.Update(t);
        await _uow.SaveChangesAsync();

        var result = ToDTOSimple(t);
        result.Tags = await GetTagDTOsAsync(dto.TagIDs ?? new List<int>());
        result.ProjectName = (await _uow.Projects.GetByIdAsync(t.ProjectID))?.ProjectName;
        return result;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var t = await _uow.Tasks.GetByIdAsync(id);
        if (t == null) return false;

        t.IsActive = false;          // soft delete, never hard-delete
        t.ModifiedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _uow.Tasks.Update(t);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TaskDTO>> SearchAsync(TaskSearchDTO c)
    {
        var tasks = await _uow.Tasks.GetAllAsync();
        var query = tasks.Where(t => t.IsActive).AsQueryable();

        if (!string.IsNullOrWhiteSpace(c.Title))
            query = query.Where(t => t.Title.ToLower().Contains(c.Title.ToLower().Trim()));
        if (c.Status.HasValue)  query = query.Where(t => t.Status == c.Status.Value);
        if (c.Priority.HasValue) query = query.Where(t => t.Priority == c.Priority.Value);
        if (c.ProjectId.HasValue) query = query.Where(t => t.ProjectID == c.ProjectId.Value);

        var result = query.ToList();

        if (c.TagId.HasValue)
        {
            var links = await _uow.TaskTags.FindAsync(tt => tt.TagID == c.TagId.Value);
            var taskIds = links.Select(l => l.TaskID).ToHashSet();
            result = result.Where(t => taskIds.Contains(t.TaskID)).ToList();
        }

        return await AddProjectNamesAsync(result);
    }

    private async Task<List<TagDTO>> GetTagDTOsAsync(List<int> tagIds)
    {
        var tags = await _uow.Tags.GetAllAsync();
        return tags.Where(x => tagIds.Contains(x.TagID))
                   .Select(x => new TagDTO { TagID = x.TagID, TagName = x.TagName, Color = x.Color })
                   .ToList();
    }

    private async Task<List<TagDTO>> GetTaskTagsAsync(int taskId)
    {
        var links = await _uow.TaskTags.FindAsync(link => link.TaskID == taskId);
        return await GetTagDTOsAsync(links.Select(link => link.TagID).ToList());
    }

    private async Task<List<TaskDTO>> AddProjectNamesAsync(IEnumerable<TaskItem> tasks)
    {
        var taskList = tasks.ToList();
        var projects = (await _uow.Projects.GetAllAsync())
            .ToDictionary(project => project.ProjectID, project => project.ProjectName);

        return taskList.Select(task =>
        {
            var dto = ToDTOSimple(task);
            dto.ProjectName = projects.GetValueOrDefault(task.ProjectID);
            return dto;
        }).ToList();
    }

    private static TaskDTO ToDTOSimple(TaskItem t) => new()
    {
        TaskID = t.TaskID, Title = t.Title, Description = t.Description,
        Status = t.Status, Priority = t.Priority, DueDate = t.DueDate,
        ProjectID = t.ProjectID, ProjectName = t.Project?.ProjectName,
        IsActive = t.IsActive, CreatedDate = t.CreatedDate, ModifiedDate = t.ModifiedDate
    };
}
