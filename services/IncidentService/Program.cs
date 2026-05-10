using IncidentService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddHostedService<RabbitMqConsumer>();
builder.Services.AddSingleton<IncidentDetectionService>();
builder.Services.AddHttpClient<AIAnalysisClient>();

var app = builder.Build();
app.UseCors("AllowFrontend");   // MUST be FIRST

app.UseHttpsRedirection();

app.MapControllers();

app.Run();