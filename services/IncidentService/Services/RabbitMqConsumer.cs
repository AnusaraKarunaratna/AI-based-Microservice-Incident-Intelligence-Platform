using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace IncidentService.Services;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IncidentDetectionService _incidentDetectionService;
    private readonly AIAnalysisClient _aiClient;
    private readonly IConfiguration _configuration;
    private readonly RedisCacheService _redis;

    public RabbitMqConsumer(
    IncidentDetectionService incidentDetectionService,
    AIAnalysisClient aiClient,
    IConfiguration configuration,
    RedisCacheService redis)
    {
        _incidentDetectionService = incidentDetectionService;
        _aiClient = aiClient;
        _configuration = configuration;
        _redis = redis;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run in background thread so it doesn't block app startup
        Task.Run(() => StartConsuming(stoppingToken), stoppingToken);
        return Task.CompletedTask;
    }

    private void StartConsuming(CancellationToken stoppingToken)
    {
        IConnection? connection = null;
        IModel? channel = null;

        // Retry loop for RabbitMQ connection
        const int maxRetries = 10;
        int retries = 0;

        while (connection == null || !connection.IsOpen)
        {
            try
            {
                Console.WriteLine($"Attempting RabbitMQ connection (attempt {retries + 1}/{maxRetries})...");

                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQ:Host"],
                    RequestedHeartbeat = TimeSpan.FromSeconds(60),
                    AutomaticRecoveryEnabled = true
                };

                connection = factory.CreateConnection();
                Console.WriteLine("RabbitMQ connected successfully.");
            }
            catch (Exception ex)
            {
                retries++;
                Console.WriteLine($"RabbitMQ connection failed: {ex.Message}");

                if (retries >= maxRetries)
                {
                    Console.WriteLine("Max RabbitMQ retries reached. Consumer will not start.");
                    return;
                }

                Console.WriteLine("Retrying in 5 seconds...");
                Thread.Sleep(5000);
            }
        }

        channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "logs_queue",
            durable: true,
            exclusive: false,
            autoDelete: false);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"Received message: {message}");

            var incident = _incidentDetectionService.AnalyzeLog(message);

            if (incident != null)
            {
                // Use Task.Run to avoid async void and properly catch exceptions
                Task.Run(async () =>
                {
                    try
                    {
                        await _aiClient.AnalyzeIncidentAsync(incident);
                        await _redis.SaveIncidentAsync(incident);
                        Console.WriteLine("Incident saved to Redis.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"AI analysis failed: {ex.Message}");
                    }
                });
            }
        };

        channel.BasicConsume(
            queue: "logs_queue",
            autoAck: true,
            consumer: consumer);

        Console.WriteLine("RabbitMQ consumer started. Listening on logs_queue...");

        // Keep the consumer alive until cancellation is requested
        while (!stoppingToken.IsCancellationRequested)
        {
            Thread.Sleep(1000);
        }

        channel?.Close();
        connection?.Close();
        Console.WriteLine("RabbitMQ consumer stopped.");
    }
}