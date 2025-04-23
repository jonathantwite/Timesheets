using AdminViewer.Services;
using AdminViewer.Services.DTOs;
using AggregatedTimeDatabase;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using Timesheets.Globals;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddRedisDistributedCache(ServiceNames.Cache);

builder.AddSqlServerDbContext<AggregatedTimeContext>(ServiceNames.AggregatedTimeDb);
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var users = app.MapGroup("Users");
users.MapGet("Missing", async (IDistributedCache cache, IUserService userService) =>
{
    var cachedUsers = await cache.GetAsync("MissingUsers");
    if (cachedUsers is null)
    {
        var users = await userService.GetMissingUsersAsync();
        await cache.SetAsync("MissingUsers", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(users)), new ()
        {
            AbsoluteExpiration = DateTime.Now.AddSeconds(10)
        });
        return Results.Ok(users);
    }
    return Results.Ok(JsonSerializer.Deserialize<IEnumerable<MissingUser>>(Encoding.UTF8.GetString(cachedUsers)));
});

users.MapPost("Add", async (IUserService userService, int userId, string name) =>
{
    await userService.AddUser(userId, name);
    return Results.Ok();
});

app.Run();
