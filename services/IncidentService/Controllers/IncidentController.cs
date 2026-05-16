using Microsoft.AspNetCore.Mvc;
using IncidentService.Services;

namespace IncidentService.Controllers;

[ApiController]
[Route("api/incidents")]
public class IncidentController : ControllerBase
{
    private readonly RedisCacheService _redis;

    public IncidentController(
        RedisCacheService redis)
    {
        _redis = redis;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var incidents =
            await _redis.GetIncidentsAsync();

        return Ok(incidents);
    }
}