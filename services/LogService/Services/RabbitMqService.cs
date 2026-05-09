using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace LogService.Services;

public class RabbitMqService
{
    public void PublishMessage(object message)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "logs_queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var jsonMessage = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        channel.BasicPublish(
            exchange: "",
            routingKey: "logs_queue",
            basicProperties: null,
            body: body);
    }
}