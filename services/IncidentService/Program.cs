using IncidentService.Services;
using StackExchange.Redis;
using Prometheus;

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

            // FIX: Add abortConnect=false so the client doesn't throw on first failed attempt
            var configOptions = ConfigurationOptions.Parse(connStr);
            configOptions.AbortOnConnectFail = false;
            configOptions.ConnectRetry = 3;
            configOptions.ConnectTimeout = 5000;

            var connection = ConnectionMultiplexer.Connect(configOptions);
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

// FIX: Pass IConfiguration to RedisCacheService so it can read the Redis endpoint
builder.Services.AddSingleton<RedisCacheService>(sp =>
{
    var redis = sp.GetRequiredService<IConnectionMultiplexer>();
    var config = sp.GetRequiredService<IConfiguration>();
    return new RedisCacheService(redis, config);
});

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

// Prometheus metrics middleware
app.UseHttpMetrics();

app.MapMetrics();

app.MapControllers();

app.Run();