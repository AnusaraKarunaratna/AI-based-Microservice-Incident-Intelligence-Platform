using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace IncidentService.Services;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IncidentDetectionService _incidentDetectionService;
    private readonly AIAnalysisClient _aiClient;
    private readonly IConfiguration _configuration;

    public RabbitMqConsumer(
        IncidentDetectionService incidentDetectionService,
        AIAnalysisClient aiClient,
        IConfiguration configuration)
    {
        _incidentDetectionService = incidentDetectionService;
        _aiClient = aiClient;
        _configuration = configuration;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"]
        };

        var connection = factory.CreateConnection();

        var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "logs_queue",
            durable: true,
            exclusive: false,
            autoDelete: false);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (_, ea) =>
        {
            var body = ea.Body.ToArray();

            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"Received: {message}");

            var incident =
                _incidentDetectionService.AnalyzeLog(message);

            if (incident != null)
            {
                await _aiClient.AnalyzeIncidentAsync(incident);
                Console.WriteLine("AI recommendation added.");
            }
        };

        channel.BasicConsume(
            queue: "logs_queue",
            autoAck: true,
            consumer: consumer);

        return Task.CompletedTask;
    }
}