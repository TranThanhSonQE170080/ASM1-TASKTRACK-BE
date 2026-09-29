using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Services;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service) => _service = service;

    /// <summary>List all active tasks</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDTO>>> GetAll()
        => Ok(await _service.GetAllAsync());

    /// <summary>Get one task including its tags</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskDTO>> GetById(int id)
    {
        var t = await _service.GetByIdAsync(id);
        return t == null ? NotFound() : Ok(t);
    }

    /// <summary>Get tasks by project</summary>
    [HttpGet("project/{projectId:int}")]
    public async Task<ActionResult<IEnumerable<TaskDTO>>> GetByProject(int projectId)
        => Ok(await _service.GetByProjectAsync(projectId));

    /// <summary>Filter tasks (all params optional)</summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<TaskDTO>>> Search(
        [FromQuery] string? title, [FromQuery] int? status, [FromQuery] int? priority,
        [FromQuery] int? projectId, [FromQuery] int? tagId)
        => Ok(await _service.SearchAsync(new TaskSearchDTO
        {
            Title = title, Status = status, Priority = priority,
            ProjectId = projectId, TagId = tagId
        }));

    /// <summary>Create a task (accept optional TagIDs array)</summary>
    [HttpPost]
    public async Task<ActionResult<TaskDTO>> Create([FromBody] TaskCreateDTO dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.TaskID }, created);
    }

    /// <summary>Update a task; replaces its tags; sets ModifiedDate to now</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskDTO>> Update(int id, [FromBody] TaskUpdateDTO dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    /// <summary>Soft-delete: set IsActive = false, never hard-delete</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await _service.SoftDeleteAsync(id) ? NoContent() : NotFound();
}
