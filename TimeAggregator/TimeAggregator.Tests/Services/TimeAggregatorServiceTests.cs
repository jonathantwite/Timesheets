using AggregatedTimeDatabase;
using AggregatedTimeDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using TestingUtilities;

namespace TimeAggregator.Services;
public class TimeAggregatorServiceTests : DbMockedTestFixture<AggregatedTimeContext>
{
    [Fact]
    public async Task AddNewTimeAsync_AddsNewJobWithEmptyTitle()
    {
        // Arrange
        using var dbContext = CreateContext();
        var service = new TimeAggregatorService(dbContext);

        // Act
        await service.AddNewTimeAsync(1, 1, DateTime.UtcNow);
        var job = await dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == 1);

        // Assert
        Assert.NotNull(job);
        Assert.Equal(1, job.Id);
        Assert.Equal("", job.Description);
    }

    [Fact]
    public async Task AddNewTimeAsync_AddsNewUserWithEmptyNameAndLastEnd()
    {
        // Arrange
        using var dbContext = CreateContext();
        var service = new TimeAggregatorService(dbContext);
        var endDate = DateTime.Today.AddHours(13).AddMinutes(23).AddSeconds(41);

        // Act
        await service.AddNewTimeAsync(1, 1, endDate);
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == 1);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(1, user.Id);
        Assert.Equal("", user.Name);
        Assert.Equal(endDate, user.LastEndTime);
    }

    [Fact]
    public async Task AddNewTimeAsync_AddsNewJonTotalWithCorrectTime()
    {
        // Arrange
        using var dbContext = CreateContext();
        var service = new TimeAggregatorService(dbContext);
        var endDate = DateTime.Today.AddHours(13).AddMinutes(23).AddSeconds(41);

        // Act
        await service.AddNewTimeAsync(1, 1, endDate);
        var jobTotal = await dbContext.JobTotals.FirstOrDefaultAsync(jt => jt.JobId == 1 && jt.UserId == 1);

        // Assert
        Assert.NotNull(jobTotal);
        Assert.Equal(1, jobTotal.JobId);
        Assert.Equal(1, jobTotal.UserId);
        Assert.Equal(endDate - DateTime.Today.AddHours(9), jobTotal.TotalTime);
    }

    [Fact]
    public async Task AddNewTimeAsync_ReusesExistingJob()
    {
        // Arrange
        using var dbContext = CreateContext();
        dbContext.Jobs.Add(new Job
        {
            Id = 1,
            Description = "Existing Job"
        });
        await dbContext.SaveChangesAsync();

        var service = new TimeAggregatorService(dbContext);

        // Act
        await service.AddNewTimeAsync(1, 1, DateTime.UtcNow);
        var job = await dbContext.Jobs.Where(j => j.Id == 1).ToListAsync();

        // Assert
        Assert.Single(job);
        Assert.Equal(1, job[0].Id);
        Assert.Equal("Existing Job", job[0].Description);
    }

    [Fact]
    public async Task CleanUpAsync_ResetsAllUsersLastEndTime()
    {
        // Arrange
        using var dbContext = CreateContext();
        dbContext.Users.Add(new User
        {
            Id = 1,
            Name = "User 1",
            LastEndTime = DateTime.Today.AddHours(13).AddMinutes(23).AddSeconds(41)
        });
        dbContext.Users.Add(new User
        {
            Id = 2,
            Name = "User 2",
            LastEndTime = DateTime.Today.AddHours(14).AddMinutes(23).AddSeconds(41)
        });
        await dbContext.SaveChangesAsync();
        var service = new TimeAggregatorService(dbContext);

        // Act
        await service.CleanUpAsync();
        var users = await dbContext.Users.ToListAsync();

        // Assert
        Assert.All(users, user => Assert.Equal(DateTime.MinValue, user.LastEndTime));
    }

    [Fact]
    public async Task CleanUpAsync_CreatesOvertimeRecords()
    {
        // Arrange
        using var dbContext = CreateContext();
        dbContext.Users.Add(new User
        {
            Id = 1,
            Name = "User 1",
            LastEndTime = DateTime.Today
                .AddHours(User.DefaultDayStartTimeHours)
                .Add(Overtime.MaxTimeBeforeOvertime)
                .AddHours(3).AddMinutes(23).AddSeconds(41)
        });
        dbContext.Users.Add(new User
        {
            Id = 2,
            Name = "User 2",
            LastEndTime = DateTime.Today
                .AddHours(User.DefaultDayStartTimeHours)
                .Add(Overtime.MaxTimeBeforeOvertime)
                .AddHours(0).AddMinutes(3).AddSeconds(18)
        });
        dbContext.Users.Add(new User
        {
            Id = 3,
            Name = "User 3",
            LastEndTime = DateTime.Today
                .AddHours(User.DefaultDayStartTimeHours)
                .Add(Overtime.MaxTimeBeforeOvertime)
                .AddHours(-2).AddMinutes(3).AddSeconds(18)
        });
        await dbContext.SaveChangesAsync();
        var service = new TimeAggregatorService(dbContext);

        // Act
        await service.CleanUpAsync();
        var overtimeRecords = await dbContext.OvertimeRecords.ToListAsync();

        // Assert
        Assert.Equal(2, overtimeRecords.Count);
        Assert.Equal(1, overtimeRecords[0].UserId);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), overtimeRecords[0].Date);
        Assert.Equal(new TimeSpan(3, 23, 41), overtimeRecords[0].OvertimeTime);

        Assert.Equal(2, overtimeRecords[1].UserId);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), overtimeRecords[1].Date);
        Assert.Equal(new TimeSpan(0, 3, 18), overtimeRecords[1].OvertimeTime);
    }
}
