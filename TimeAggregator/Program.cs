using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Messaging.Shared.Constants;
using TimeAdder.Api.Contracts.Messages;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();
builder.AddRabbitMQClient(connectionName: "messaging");

using IHost host = builder.Build();


var _rabbitConnection = host.Services.GetRequiredService<IConnection>();

var channel = _rabbitConnection.CreateModel();
channel.ExchangeDeclare(exchange: MessagingConstants.NewTimeRecordedExchange, type: ExchangeType.Fanout);

var queueName = channel.QueueDeclare().QueueName;

channel.QueueBind(queue: queueName, exchange: MessagingConstants.NewTimeRecordedExchange, routingKey: string.Empty);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += ProcessMessageAsync;

channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

Console.ReadLine();

//await host.RunAsync();

void ProcessMessageAsync(object? sender, BasicDeliverEventArgs args)
{
    string messagetext = Encoding.UTF8.GetString(args.Body.ToArray());

    var recordTimeMessage = JsonSerializer.Deserialize<RecordTimeMessage>(messagetext);
    Console.WriteLine("The message is: " + JsonSerializer.Serialize(recordTimeMessage));

    // here channel could also be accessed as ((AsyncEventingBasicConsumer)sender).Channel
    channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
}