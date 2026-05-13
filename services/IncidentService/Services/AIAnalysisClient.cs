using System.Text;
using System.Text.Json;
using IncidentService.Models;

namespace IncidentService.Services;

public class AIAnalysisClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AIAnalysisClient(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task AnalyzeIncidentAsync(Incident incident)
    {
        var json = JsonSerializer.Serialize(incident);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var baseUrl = _configuration["AIService:Url"];

        var response = await _httpClient.PostAsync(
            $"{baseUrl}/api/analysis",
            content);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadAsStringAsync();

        var analysis =
            JsonSerializer.Deserialize<AIAnalysisResponse>(
                result,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (analysis != null)
        {
            incident.RootCause = analysis.RootCause;
            incident.Recommendation = analysis.Recommendation;
            incident.Priority = analysis.Priority;
        }

        Console.WriteLine("\nAI ANALYSIS:");
        Console.WriteLine(result);
    }
}

public class AIAnalysisResponse
{
    public string RootCause { get; set; } = string.Empty;

    public string Recommendation { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;
}