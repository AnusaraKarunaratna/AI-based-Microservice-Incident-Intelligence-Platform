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
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"]
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
    }
}