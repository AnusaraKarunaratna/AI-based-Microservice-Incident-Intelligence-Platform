using System.Collections.Concurrent;
using IncidentService.Models;

namespace IncidentService.Services;

public class IncidentDetectionService
{
    // Store error counts per service
    private static readonly ConcurrentDictionary<string, int> ErrorTracker = new();

    // Threshold
    private const int ERROR_THRESHOLD = 5;

    public Incident? AnalyzeLog(string logMessage)
    {
        // Simple detection logic
        if (!logMessage.ToUpper().Contains("ERROR"))
            return null;

        // Extract service name manually
        string serviceName = "unknown-service";

        if (logMessage.Contains("payment-service"))
            serviceName = "payment-service";

        if (logMessage.Contains("auth-service"))
            serviceName = "auth-service";

        // Increment count
        ErrorTracker.AddOrUpdate(
            serviceName,
            1,
            (key, current) => current + 1);

        int currentCount = ErrorTracker[serviceName];

        Console.WriteLine(
            $"{serviceName} error count: {currentCount}");

        // Detect incident
        if (currentCount >= ERROR_THRESHOLD)
        {
            return new Incident
            {
                ServiceName = serviceName,
                ErrorCount = currentCount,
                Severity = "HIGH",
                Message = $"{serviceName} exceeded error threshold"
            };
        }

        return null;
    }
}