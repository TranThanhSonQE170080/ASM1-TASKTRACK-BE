using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Services;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectsController(IProjectService service) => _service = service;

    /// <summary>List all active projects (include department name)</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetAll()
        => Ok(await _service.GetAllAsync());

    /// <summary>Get one project and its tasks</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDetailDTO>> GetById(int id)
    {
        var p = await _service.GetByIdAsync(id);
        return p == null ? NotFound() : Ok(p);
    }

    /// <summary>Get projects by department</summary>
    [HttpGet("department/{departmentId:int}")]
    public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetByDepartment(int departmentId)
        => Ok(await _service.GetByDepartmentAsync(departmentId));

    /// <summary>Filter projects (all params optional)</summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProjectDTO>>> Search(
        [FromQuery] string? name, [FromQuery] int? status, [FromQuery] int? departmentId)
        => Ok(await _service.SearchAsync(new ProjectSearchDTO
        {
            Name = name, Status = status, DepartmentId = departmentId
        }));

    /// <summary>Create a new project</summary>
    [HttpPost]
    public async Task<ActionResult<ProjectDTO>> Create([FromBody] ProjectCreateDTO dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.ProjectID }, created);
    }

    /// <summary>Update a project</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectDTO>> Update(int id, [FromBody] ProjectUpdateDTO dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    /// <summary>Delete a project (only if no tasks linked)</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (deleted) return NoContent();

        var p = await _service.GetByIdAsync(id);
        return p == null
            ? NotFound()
            : BadRequest(new { message = "Cannot delete: this project still has linked tasks." });
    }
}
