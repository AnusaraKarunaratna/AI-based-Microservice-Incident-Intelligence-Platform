using Microsoft.AspNetCore.Mvc;
using IncidentService.Services;

namespace IncidentService.Controllers;

[ApiController]
[Route("api/incidents")]
public class IncidentController : ControllerBase
{
    private readonly IncidentDetectionService _service;

    public IncidentController(IncidentDetectionService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_service.Incidents);
    }
}