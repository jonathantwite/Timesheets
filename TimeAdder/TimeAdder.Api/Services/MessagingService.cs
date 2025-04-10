using Messaging.Shared.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace TimeAdder.Api.Services;

public class MessagingService(IConnection rabbitConnection) : IMessagingService
{
    private readonly IConnection _rabbitConnection = rabbitConnection ?? throw new ArgumentNullException(nameof(rabbitConnection));

    public void SendMessage<T>(string exchange, T message) where T : IMessage
    {
        var channel = _rabbitConnection.CreateModel();
        channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        channel.BasicPublish(
            exchange: exchange,
            routingKey: string.Empty,
            body: body);
    }
}
