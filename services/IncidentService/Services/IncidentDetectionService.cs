using System.Collections.Concurrent;
using System.Text.Json;
using IncidentService.Models;

namespace IncidentService.Services;

public class IncidentDetectionService
{
    private static readonly ConcurrentDictionary<string, int> ErrorTracker = new();

    private const int ERROR_THRESHOLD = 5;

    public List<Incident> Incidents { get; } = new();

    public Incident? AnalyzeLog(string logMessage)
    {
        LogEntryDto? log;

        try
        {
            log = JsonSerializer.Deserialize<LogEntryDto>(logMessage);
        }
        catch
        {
            return null;
        }

        if (log == null)
            return null;

        string message = log.Message.ToUpper();

        if (!message.Contains("ERROR") &&
            !message.Contains("TIMEOUT") &&
            !message.Contains("FAIL"))
        {
            return null;
        }

        string serviceName = log.ServiceName ?? "unknown-service";

        ErrorTracker.AddOrUpdate(
            serviceName,
            1,
            (key, current) => current + 1);

        int count = ErrorTracker[serviceName];

        if (count >= ERROR_THRESHOLD)
        {
            var incident = new Incident
            {
                ServiceName = serviceName,
                ErrorCount = count,
                Severity = "HIGH",
                Message = $"{serviceName} exceeded error threshold"
            };

            Incidents.Add(incident);

            Console.WriteLine("INCIDENT CREATED");

            return incident;
        }

        return null;
    }
}