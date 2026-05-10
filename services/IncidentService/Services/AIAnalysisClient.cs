using System.Text;
using System.Text.Json;

namespace IncidentService.Services;

public class AIAnalysisClient
{
    private readonly HttpClient _httpClient;

    public AIAnalysisClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task AnalyzeIncidentAsync(object incident)
    {
        var json = JsonSerializer.Serialize(incident);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(
            "http://localhost:5247/api/analysis",
            content);

        var result =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine("\nAI ANALYSIS RESULT:");
        Console.WriteLine(result);
    }
}