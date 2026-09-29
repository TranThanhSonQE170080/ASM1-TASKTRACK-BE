using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Services;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _service;

    public DepartmentsController(IDepartmentService service) => _service = service;

    /// <summary>List all active departments</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentDTO>>> GetAll()
        => Ok(await _service.GetAllAsync());

    /// <summary>Get one department and its projects</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentDetailDTO>> GetById(int id)
    {
        var dept = await _service.GetByIdAsync(id);
        return dept == null ? NotFound() : Ok(dept);
    }

    /// <summary>Search departments by name (partial match)</summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<DepartmentDTO>>> Search([FromQuery] string name)
        => Ok(await _service.SearchByNameAsync(name));

    /// <summary>Create a new department</summary>
    [HttpPost]
    public async Task<ActionResult<DepartmentDTO>> Create([FromBody] DepartmentCreateDTO dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.DepartmentID }, created);
    }

    /// <summary>Update a department</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<DepartmentDTO>> Update(int id, [FromBody] DepartmentUpdateDTO dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    /// <summary>Delete a department (only if no projects linked)</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (deleted) return NoContent();

        // distinguish: not found vs. has linked projects
        var dept = await _service.GetByIdAsync(id);
        return dept == null
            ? NotFound()
            : BadRequest(new { message = "Cannot delete: this department still has linked projects." });
    }
}
