using Microsoft.AspNetCore.Mvc;
using LogService.Data;
using LogService.Models;
using LogService.Services;

namespace LogService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly RabbitMqService _rabbitMqService;

    public LogsController(
        ApplicationDbContext context,
        RabbitMqService rabbitMqService)
    {
        _context = context;
        _rabbitMqService = rabbitMqService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLog(LogEntry log)
    {
        _context.Logs.Add(log);

        await _context.SaveChangesAsync();

        // Publish to RabbitMQ
        _rabbitMqService.PublishMessage(log);

        return Ok(log);
    }

    [HttpGet]
    public IActionResult GetLogs()
    {
        return Ok(_context.Logs.ToList());
    }
}