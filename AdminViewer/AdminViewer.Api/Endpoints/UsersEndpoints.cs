using AdminViewer.Models.Requests;
using AdminViewer.Models.Responses;
using AdminViewer.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace AdminViewer.Api.Endpoints;

public static class UsersEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var users = app.MapGroup("Users");
        users.MapGet("Missing", GetMissingUsersAsync);
        users.MapPost("Add", AddUser);
    }

    public static async Task<Results<Ok<IEnumerable<MissingUser>>, NotFound>> GetMissingUsersAsync(this IUserService userService, IDistributedCache cache)
    {
        var cachedUsers = await cache.GetAsync("MissingUsers");
        if (cachedUsers is null)
        {
            var users = await userService.GetMissingUsersAsync();
            await cache.SetAsync("MissingUsers", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(users)), new()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(10)
            });
            return TypedResults.Ok(users);
        }
        return TypedResults.Ok(JsonSerializer.Deserialize<IEnumerable<MissingUser>>(Encoding.UTF8.GetString(cachedUsers)));
    }

    public static async Task<Results<Ok, BadRequest<ValidationResult>>> AddUser(IUserService userService, IValidator<AddUserRequest> validator, [FromBody] AddUserRequest user)
    {
        var errors = await validator.ValidateAsync(user);
        if (!errors.IsValid)
        {
            return TypedResults.BadRequest(errors);
        }
        await userService.AddUser(user);
        return TypedResults.Ok();
    }
}
