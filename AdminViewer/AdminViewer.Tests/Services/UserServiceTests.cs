using AggregatedTimeDatabase;
using AggregatedTimeDatabase.Entities;
using TestingUtilities;

namespace AdminViewer.Services
{
    public class UserServiceTests: DbMockedTestFixture<AggregatedTimeContext>
    {
        // Test UserService.GetMissingUsersAsync
        [Fact]
        public async Task GetMissingUsersAsync_ShouldReturnMissingUsers()
        {
            // Arrange
            var dbContext = CreateContext();
            var userService = new UserService(dbContext);

            var job = new Job
            {
                Id = 1,
                Description = "Description 1"
            };

            var user = new User
            {
                Id = 1,
                Name = "",
                LastEndTime = DateTime.Today.AddHours(9)
            };

            var jobTotal = new JobTotal
            {
                JobId = 1,
                UserId = 1,
                TotalTime = TimeSpan.FromHours(1)
            };

            dbContext.Jobs.Add(job);
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await userService.GetMissingUsersAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(user.Id, result.First().Id);
        }

        [Fact]
        public async Task GetMissingUsersAsync_ShouldNotReturnUsersWithNames()
        {
            // Arrange
            var dbContext = CreateContext();
            var userService = new UserService(dbContext);
            var user = new User
            {
                Id = 1,
                Name = "Test User",
                LastEndTime = DateTime.Today.AddHours(9)
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await userService.GetMissingUsersAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetMissingUsersAsync_ShouldSumUpJobTotalsForUser()
        {
            // Arrange
            var dbContext = CreateContext();
            var userService = new UserService(dbContext);
            var job1 = new Job { Id = 1, Description = "Job 1" };
            var job2 = new Job { Id = 2, Description = "Job 2" };
            var user = new User
            {
                Id = 1,
                Name = "",
                LastEndTime = DateTime.Today.AddHours(9),
                JobTotals =
                {
                    new JobTotal { JobId = 1, UserId = 1, TotalTime = TimeSpan.FromHours(2) },
                    new JobTotal { JobId = 2, UserId = 1, TotalTime = TimeSpan.FromHours(3) }
                }
            };
            dbContext.Jobs.Add(job1);
            dbContext.Jobs.Add(job2);
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await userService.GetMissingUsersAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(TimeSpan.FromHours(5), result.First().TotalTimeRecorded);
        }
    }
}
