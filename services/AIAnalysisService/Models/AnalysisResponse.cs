namespace AIAnalysisService.Models;

public class AnalysisResponse
{
    public string RootCause { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
}