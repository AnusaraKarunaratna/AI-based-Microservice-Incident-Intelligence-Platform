using IncidentService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<RabbitMqConsumer>();

builder.Services.AddSingleton<IncidentDetectionService>();

var app = builder.Build();

app.MapGet("/", () => "Incident Service Running");

app.Run();