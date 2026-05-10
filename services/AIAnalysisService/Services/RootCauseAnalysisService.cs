using AIAnalysisService.Models;

namespace AIAnalysisService.Services;

public class RootCauseAnalysisService
{
    public AnalysisResponse Analyze(IncidentRequest incident)
    {
        var response = new AnalysisResponse();

        // Payment Service Analysis
        if (incident.ServiceName.Contains("payment"))
        {
            response.RootCause = "Possible Redis cache timeout or database latency issue.";
            response.Recommendation = "Check Redis health, database response time, and retry policies.";
            response.Priority = "HIGH";
        }
        // Auth Service Analysis
        else if (incident.ServiceName.Contains("auth"))
        {
            response.RootCause = "Authentication token validation failures detected.";
            response.Recommendation = "Verify JWT signing keys and token expiration configuration.";
            response.Priority = "MEDIUM";
        }
        // Inventory Service Analysis
        else if (incident.ServiceName.Contains("inventory"))
        {
            response.RootCause = "Inventory synchronization delays detected.";
            response.Recommendation = "Check message queue processing and stock update workflows.";
            response.Priority = "HIGH";
        }
        // Default
        else
        {
            response.RootCause = "Unknown distributed system anomaly detected.";
            response.Recommendation = "Inspect logs and service communication flow.";
            response.Priority = "MEDIUM";
        }
        return response;
    }
}