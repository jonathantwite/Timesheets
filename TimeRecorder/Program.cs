using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();
builder.AddRabbitMQClient(connectionName: "messaging");

using IHost host = builder.Build();


var _rabbitConnection = host.Services.GetRequiredService<IConnection>();

var queue = "TimeAdded";

var channel = _rabbitConnection.CreateModel();
channel.QueueDeclare(
    queue: queue,
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += ProcessMessageAsync;

channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);

Console.ReadLine();

//await host.RunAsync();

void ProcessMessageAsync(object? sender, BasicDeliverEventArgs args)
{
    string messagetext = Encoding.UTF8.GetString(args.Body.ToArray());
    Console.WriteLine("The message is: " + messagetext);
}