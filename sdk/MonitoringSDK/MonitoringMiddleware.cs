using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace MonitoringSDK;

public class MonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HttpClient _httpClient;
    private readonly MonitoringOptions _options;

    public MonitoringMiddleware(
        RequestDelegate next,
        MonitoringOptions options)
    {
        _next = next;
        _options = options;

        _httpClient = new HttpClient();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            // Capture 500 errors
            if (context.Response.StatusCode >= 500)
            {
                await SendLog(
                    $"HTTP {context.Response.StatusCode} server error",
                    "ERROR");
            }
        }
        catch (Exception ex)
        {
            await SendLog(
                ex.Message,
                "CRITICAL");

            throw;
        }
    }

    private async Task SendLog(
        string message,
        string level)
    {
        var log = new
        {
            ServiceName = _options.ServiceName,
            Level = level,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(log);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        try
        {
            await _httpClient.PostAsync(
                $"{_options.ApiUrl}/api/logs",
                content);
        }
        catch
        {
            // Prevent monitoring crashes
        }
    }
}