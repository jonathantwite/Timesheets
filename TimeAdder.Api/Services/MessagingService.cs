using Messaging.Shared.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace TimeAdder.Api.Services;

public class MessagingService(IConnection rabbitConnection) : IMessagingService
{
    private readonly IConnection _rabbitConnection = rabbitConnection ?? throw new ArgumentNullException(nameof(rabbitConnection));

    public void SendMessage<T>(string queue, T message) where T : IMessage
    {
        var channel = _rabbitConnection.CreateModel();
        channel.QueueDeclare(
            queue: queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: queue,
            basicProperties: null,
            body: body);
    }
}
