using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MonitoringSDK;

public static class MonitoringExtensions
{
    public static IServiceCollection
        AddIncidentMonitoring(
            this IServiceCollection services,
            Action<MonitoringOptions> configure)
    {
        var options = new MonitoringOptions();

        configure(options);

        services.AddSingleton(options);

        return services;
    }

    public static IApplicationBuilder
        UseIncidentMonitoring(
            this IApplicationBuilder app)
    {
        return app.UseMiddleware<MonitoringMiddleware>();
    }
}