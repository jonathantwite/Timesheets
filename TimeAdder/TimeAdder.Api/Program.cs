using Scalar.AspNetCore;
using TimeAdder.Api.Endpoints;
using TimeAdder.Api.Services;
using Timesheets.Globals;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddRabbitMQClient(connectionName: ServiceNames.RabbitMQ);

builder.Services.AddScoped<IMessagingService, MessagingService>();
builder.Services.AddScoped<ITimeRequestService, TimeRequestService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapTimeEndpoints();

app.Run();


public partial class Program { }