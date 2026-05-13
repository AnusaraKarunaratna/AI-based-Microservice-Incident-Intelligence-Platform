namespace IncidentService.Models;

public class Incident
{
    public string ServiceName { get; set; } = string.Empty;

    public int ErrorCount { get; set; }

    public string Severity { get; set; } = string.Empty;

    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;

    public string Message { get; set; } = string.Empty;

    // AI Analysis
    public string RootCause { get; set; } = string.Empty;

    public string Recommendation { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;
}