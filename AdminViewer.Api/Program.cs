using AdminViewer.Api.Endpoints;
using AdminViewer.Models.Validators;
using AdminViewer.Services;
using AggregatedTimeDatabase;
using FluentValidation;
using Timesheets.Globals;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddRedisDistributedCache(ServiceNames.Cache);

builder.AddSqlServerDbContext<AggregatedTimeContext>(ServiceNames.AggregatedTimeDb);

builder.Services.AddValidatorsFromAssemblyContaining<AddUserRequestValidator>(ServiceLifetime.Transient);

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapUserEndpoints();

app.Run();
