using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Messaging.Shared.Constants;
using TimeAdder.Api.Contracts.Messages;
using AggregatedTimeDatabase;
using TimeAggregator.Services;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();
//builder.AddRabbitMQClient(connectionName: "messaging", configureConnectionFactory: factory => { factory.DispatchConsumersAsync = true; });
builder.AddRabbitMQClient(connectionName: "messaging");
builder.AddSqlServerDbContext<AggregatedTimeContext>(connectionName: "AggregatedTimeDb");
builder.Services.AddScoped<ITimeAggregatorService, TimeAggregatorService>();

using IHost host = builder.Build();

var _rabbitConnection = host.Services.GetRequiredService<IConnection>();

var channel = _rabbitConnection.CreateModel();
channel.ExchangeDeclare(exchange: MessagingConstants.NewTimeRecordedExchange, type: ExchangeType.Fanout);

var queueName = channel.QueueDeclare().QueueName;

channel.QueueBind(queue: queueName, exchange: MessagingConstants.NewTimeRecordedExchange, routingKey: string.Empty);

//var consumer = new AsyncEventingBasicConsumer(channel);
var consumer = new EventingBasicConsumer(channel);
consumer.Received += ProcessMessageAsync;

channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.ReadLine();

//await host.RunAsync();

void ProcessMessageAsync(object? sender, BasicDeliverEventArgs args)
{
    string messagetext = Encoding.UTF8.GetString(args.Body.ToArray());

    var recordTimeMessage = JsonSerializer.Deserialize<RecordTimeMessage>(messagetext);
    Console.WriteLine("The message is: " + JsonSerializer.Serialize(recordTimeMessage));

    if(recordTimeMessage == null)
    {
        return;
    }

    var service = host.Services.GetRequiredService<ITimeAggregatorService>();
    Task.Run(async () =>
    {
        await service.AddNewTimeAsync(recordTimeMessage.UserId, recordTimeMessage.JobId, recordTimeMessage.EndTime);
    });

    // here channel could also be accessed as ((AsyncEventingBasicConsumer)sender).Channel
    //channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
}