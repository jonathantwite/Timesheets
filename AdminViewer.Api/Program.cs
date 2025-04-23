using AdminViewer.Services;
using AggregatedTimeDatabase;
using Timesheets.Globals;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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
users.MapGet("Missing", async (IUserService userService) =>
{
    var users = await userService.GetMissingUsersAsync();
    return Results.Ok(users);
});
users.MapPost("Add", async (IUserService userService, int userId, string name) =>
{
    await userService.AddUser(userId, name);
    return Results.Ok();
});

app.Run();
