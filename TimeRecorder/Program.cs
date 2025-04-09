using Messaging.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RawTimeEntriesDatabase;
using System.Text;
using TimeAdder.Api.Contracts.Messages;
using TimeRecorder.Services;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();
builder.AddRabbitMQClient(connectionName: "messaging");
builder.AddSqlServerDbContext<RawTimeEntriesContext>(connectionName: "sqlDbServer");
builder.Services.AddScoped<IProcessNewTimeEntryService, ProcessNewTimeEntryService>();

using IHost host = builder.Build();

var _rabbitConnection = host.Services.GetRequiredService<IConnection>();

var channel = _rabbitConnection.CreateModel();
channel.ExchangeDeclare(exchange: MessagingConstants.NewTimeRecordedExchange, type: ExchangeType.Fanout);

var queueName = channel.QueueDeclare(queue: "TimeRecorderQueue", exclusive: false).QueueName;

channel.QueueBind(queue: queueName, exchange: MessagingConstants.NewTimeRecordedExchange, routingKey: string.Empty);

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

    var service = host.Services.GetRequiredService<IProcessNewTimeEntryService>();
    Task.Run(async () => {
        await service.ProcessAsync(recordTimeMessage);

        // here channel could also be accessed as ((AsyncEventingBasicConsumer)sender).Channel
        //channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
    });
}