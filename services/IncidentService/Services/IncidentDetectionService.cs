using System.Collections.Concurrent;
using System.Text.Json;
using IncidentService.Models;

namespace IncidentService.Services;

public class IncidentDetectionService
{
    // Tracks cumulative error count per service
    private static readonly ConcurrentDictionary<string, int> ErrorTracker = new();

    // Tracks which services already have an active incident (prevents duplicate incidents)
    private static readonly ConcurrentDictionary<string, bool> ActiveIncidents = new();

    private const int ERROR_THRESHOLD = 5;

    public List<Incident> Incidents { get; } = new();

    private readonly RedisCacheService _redis;

    public IncidentDetectionService(RedisCacheService redis)
    {
        _redis = redis;
    }

    public Incident? AnalyzeLog(string logMessage)
    {
        LogEntryDto? log;

        try
        {
            log = JsonSerializer.Deserialize<LogEntryDto>(logMessage,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[IncidentDetection] Failed to deserialize log: {ex.Message}");
            Console.WriteLine($"[IncidentDetection] Raw message: {logMessage}");
            return null;
        }

        if (log == null)
        {
            Console.WriteLine("[IncidentDetection] Log deserialized to null.");
            return null;
        }

        Console.WriteLine($"[IncidentDetection] Processing — Service: {log.ServiceName}, Level: {log.Level}, Message: {log.Message}");

        string message = (log.Message ?? "").ToUpper();
        string level = (log.Level ?? "").ToUpper();

        bool isError = message.Contains("ERROR") ||
                       message.Contains("TIMEOUT") ||
                       message.Contains("FAIL") ||
                       level == "ERROR" ||
                       level == "CRITICAL";

        if (!isError)
        {
            Console.WriteLine($"[IncidentDetection] Not an error-level log, skipping.");
            return null;
        }

        string serviceName = log.ServiceName ?? "unknown-service";

        int count = ErrorTracker.AddOrUpdate(serviceName, 1, (_, current) => current + 1);

        Console.WriteLine($"[IncidentDetection] Error count for {serviceName}: {count}/{ERROR_THRESHOLD}");

        if (count >= ERROR_THRESHOLD)
        {
            // Only create ONE incident per service — just update count if already exists
            bool alreadyHasIncident = ActiveIncidents.GetOrAdd(serviceName, false);

            if (alreadyHasIncident)
            {
                var existing = Incidents.FirstOrDefault(i => i.ServiceName == serviceName);
                if (existing != null)
                {
                    existing.ErrorCount = count;
                    Console.WriteLine($"[IncidentDetection] Updated incident for {serviceName}, count now {count}.");
                }
                return null;
            }

            // First time threshold is crossed — create the incident
            ActiveIncidents[serviceName] = true;

            var incident = new Incident
            {
                ServiceName = serviceName,
                ErrorCount = count,
                Severity = "HIGH",
                Message = $"{serviceName} exceeded error threshold with {count} errors detected"
            };

            Incidents.Add(incident);

            Console.WriteLine($"[IncidentDetection] *** INCIDENT CREATED for {serviceName} ***");

            return incident;
        }

        return null;
    }

    /// <summary>
    /// Resolves an active incident for a service, allowing it to re-trigger if errors continue.
    /// </summary>
    public async Task ResolveIncident(string serviceName)
    {
        ActiveIncidents[serviceName] = false;
        ErrorTracker[serviceName] = 0;
        await _redis.RemoveIncidentAsync(serviceName);
        Console.WriteLine($"Incident resolved for {serviceName}");
    }
}