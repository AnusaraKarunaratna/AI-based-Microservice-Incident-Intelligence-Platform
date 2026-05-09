using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace IncidentService.Services;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IncidentDetectionService _incidentDetectionService;

    public RabbitMqConsumer(
        IncidentDetectionService incidentDetectionService)
    {
        _incidentDetectionService = incidentDetectionService;
    }

    protected override Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
        };

        var connection = factory.CreateConnection();

        var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "logs_queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();

            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine("\nReceived Log:");
            Console.WriteLine(message);

            // Analyze log
            var incident =
                _incidentDetectionService.AnalyzeLog(message);

            // Incident detected
            if (incident != null)
            {
                Console.WriteLine("\n====================");

                Console.WriteLine("INCIDENT DETECTED");

                Console.WriteLine(
                    $"Service: {incident.ServiceName}");

                Console.WriteLine(
                    $"Severity: {incident.Severity}");

                Console.WriteLine(
                    $"Message: {incident.Message}");

                Console.WriteLine("====================\n");
            }
        };

        channel.BasicConsume(
            queue: "logs_queue",
            autoAck: true,
            consumer: consumer);

        return Task.CompletedTask;
    }
}