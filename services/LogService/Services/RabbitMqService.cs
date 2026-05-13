using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace LogService.Services;

public class RabbitMqService
{
    private readonly IConfiguration _configuration;

    public RabbitMqService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void PublishMessage(object message)
    {
        const int maxRetries = 5;
        int retries = 0;

        while (true)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQ:Host"],
                    RequestedHeartbeat = TimeSpan.FromSeconds(60)
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(
                    queue: "logs_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(
                    exchange: "",
                    routingKey: "logs_queue",
                    body: body);

                Console.WriteLine($"Published message to logs_queue: {json}");
                return;
            }
            catch (Exception ex)
            {
                retries++;
                Console.WriteLine($"RabbitMQ publish failed (attempt {retries}/{maxRetries}): {ex.Message}");

                if (retries >= maxRetries)
                {
                    Console.WriteLine("Max publish retries reached. Message dropped.");
                    return;
                }

                Thread.Sleep(2000);
            }
        }
    }
}