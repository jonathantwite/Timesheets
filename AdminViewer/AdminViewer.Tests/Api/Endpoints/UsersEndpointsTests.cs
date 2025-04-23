using AdminViewer.Models.Requests;
using AdminViewer.Models.Responses;
using AdminViewer.Models.Validators;
using AdminViewer.Services;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using System.Text;
using System.Text.Json;

namespace AdminViewer.Api.Endpoints
{
    public class UsersEndpointsTests
    {
        // Test GetMissingUsersAsync
        [Fact]
        public async Task GetMissingUsersAsync_ShouldReturnCachedUsers_WhenCacheIsNotNull()
        {
            // Arrange
            var userService = Substitute.For<IUserService>();
            var cache = Substitute.For<IDistributedCache>();
            var users = new List<MissingUser>
            {
                new (1, DateTime.Today.AddHours(9), new TimeSpan(1,15,32), new List<string> {"Job 1", "Job 2" }),
                new (2, DateTime.Today.AddHours(9), new TimeSpan(1,15,32), new List<string> { "Job 1", "Job 2" })
            };

            var cachedData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(users));
            cache.GetAsync("MissingUsers").Returns(cachedData);

            // Act
            var result = (await UsersEndpoints.GetMissingUsersAsync(userService, cache)).Result;

            // Assert
            Assert.IsType<Ok<IEnumerable<MissingUser>>>(result);
            var okResult = result as Ok<IEnumerable<MissingUser>>;
            Assert.Equivalent(users, okResult?.Value);
        }

        // Test GetMissingUsersAsync When Cache is Null
        [Fact]
        public async Task GetMissingUsersAsync_ShouldReturnUsers_WhenCacheIsNull()
        {
            // Arrange
            var userService = Substitute.For<IUserService>();
            var cache = Substitute.For<IDistributedCache>();
            var users = new List<MissingUser>
            {
                new (1, DateTime.Today.AddHours(9), new TimeSpan(1, 15, 32), new List<string> { "Job 1", "Job 2" }),
                new (2, DateTime.Today.AddHours(9), new TimeSpan(1, 15, 32), new List<string> { "Job 1", "Job 2" })
            };
            cache.GetAsync("MissingUsers").Returns((byte[])null);
            userService.GetMissingUsersAsync().Returns(users);

            // Act
            var result = (await UsersEndpoints.GetMissingUsersAsync(userService, cache)).Result;

            // Assert
            Assert.IsType<Ok<IEnumerable<MissingUser>>>(result);
            var okResult = result as Ok<IEnumerable<MissingUser>>;
            Assert.Equivalent(users, okResult?.Value);
        }

        // Test AddUser
        [Fact]
        public async Task AddUser_ShouldReturnOk_WhenUserIsValid()
        {
            // Arrange
            var userService = Substitute.For<IUserService>();
            var user = new AddUserRequest(1, "Test User");

            // Act
            var result = (await UsersEndpoints.AddUser(userService, new AddUserRequestValidator(), user)).Result;

            // Assert
            Assert.IsType<Ok>(result);
            await userService.Received(1).AddUser(user);
        }

        [Fact]
        public async Task AddUser_ShouldReturnBadRequest_WhenUserIsInvalid()
        {
            // Arrange
            var userService = Substitute.For<IUserService>();
            var user = new AddUserRequest(1, "");
            var validator = new AddUserRequestValidator();
            var errors = await validator.ValidateAsync(user);

            // Act
            var result = (await UsersEndpoints.AddUser(userService, validator, user)).Result;

            // Assert
            Assert.IsType<BadRequest<ValidationResult>>(result);
            var badRequestResult = result as BadRequest<ValidationResult>;
            Assert.Equivalent(errors, badRequestResult?.Value);
        }
    }
}
