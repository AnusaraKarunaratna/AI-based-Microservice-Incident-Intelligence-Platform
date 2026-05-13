using IncidentService.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IncidentDetectionService>();

builder.Services.AddHostedService<RabbitMqConsumer>();

builder.Services.AddHttpClient<AIAnalysisClient>();

// Resilient Redis connection with retry logic
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var connStr = builder.Configuration["Redis:Connection"]!;
    var retries = 0;
    const int maxRetries = 5;

    while (true)
    {
        try
        {
            Console.WriteLine($"Connecting to Redis at {connStr}...");
            var connection = ConnectionMultiplexer.Connect(connStr);
            Console.WriteLine("Redis connected successfully.");
            return connection;
        }
        catch (Exception ex)
        {
            retries++;
            Console.WriteLine($"Redis connection failed (attempt {retries}/{maxRetries}): {ex.Message}");

            if (retries >= maxRetries)
            {
                Console.WriteLine("Max Redis retries reached. Throwing exception.");
                throw;
            }

            Console.WriteLine("Retrying in 3 seconds...");
            Thread.Sleep(3000);
        }
    }
});

builder.Services.AddSingleton<RedisCacheService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();