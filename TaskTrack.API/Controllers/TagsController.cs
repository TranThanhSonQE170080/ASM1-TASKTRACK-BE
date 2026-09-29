using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Services;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _service;

    public TagsController(ITagService service) => _service = service;

    /// <summary>List all tags</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDTO>>> GetAll()
        => Ok(await _service.GetAllAsync());

    /// <summary>Create a tag</summary>
    [HttpPost]
    public async Task<ActionResult<TagDTO>> Create([FromBody] TagCreateDTO dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetAll), created);
    }

    /// <summary>Update a tag</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TagDTO>> Update(int id, [FromBody] TagUpdateDTO dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    /// <summary>Delete a tag (only if not used by any task)</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (deleted) return NoContent();

        var tags = await _service.GetAllAsync();
        return tags.Any(t => t.TagID == id)
            ? BadRequest(new { message = "Cannot delete: this tag is used by one or more tasks." })
            : NotFound();
    }
}
