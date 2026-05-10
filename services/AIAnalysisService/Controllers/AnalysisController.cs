using Microsoft.AspNetCore.Mvc;
using AIAnalysisService.Models;
using AIAnalysisService.Services;   

namespace AIAnalysisService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalysisController : ControllerBase
{
    private readonly RootCauseAnalysisService _analysisService;

    public AnalysisController (RootCauseAnalysisService analysisService)
    {
        _analysisService = analysisService;
    }

    [HttpPost]
    public IActionResult AnalyzeIncident(IncidentRequest incident)
    {
        var result = _analysisService.Analyze(incident);
        return Ok(result);
    }
}