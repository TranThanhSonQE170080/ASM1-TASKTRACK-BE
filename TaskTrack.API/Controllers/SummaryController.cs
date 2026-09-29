using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Services;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SummaryController : ControllerBase
{
    private readonly ISummaryService _service;

    public SummaryController(ISummaryService service) => _service = service;

    /// <summary>Summary counts for the home page</summary>
    [HttpGet]
    public async Task<ActionResult<SummaryDTO>> Get()
        => Ok(await _service.GetSummaryAsync());
}
