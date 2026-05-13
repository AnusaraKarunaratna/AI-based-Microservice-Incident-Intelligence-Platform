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
        var baseUrl = _configuration["AIService:Url"];

        // Use camelCase to match the C# AIAnalysisService FastAPI-style endpoint
        var json = JsonSerializer.Serialize(incident, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Console.WriteLine($"\nSending to AI service at {baseUrl}/api/analysis:");
        Console.WriteLine(json);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response;

        try
        {
            response = await _httpClient.PostAsync($"{baseUrl}/api/analysis", content);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AI service request failed: {ex.Message}");
            throw;
        }

        var result = await response.Content.ReadAsStringAsync();

        Console.WriteLine("\nAI ANALYSIS RESPONSE:");
        Console.WriteLine(result);

        var analysis = JsonSerializer.Deserialize<AIAnalysisResponse>(
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
    }
}

public class AIAnalysisResponse
{
    public string RootCause { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
}