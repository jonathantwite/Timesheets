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
    public async Task AddNewTimeAsync_AddsNewJobTotalWithCorrectTime()
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
    public async Task AddNewTimeAsync_AddsMultipleNewJobTotalWithCorrectTime()
    {
        // Arrange
        using var dbContext = CreateContext();
        var service = new TimeAggregatorService(dbContext);
        var endDate1 = DateTime.Today.AddHours(9).AddMinutes(2).AddSeconds(45);
        var endDate2 = DateTime.Today.AddHours(11).AddMinutes(11).AddSeconds(22);
        var endDate3 = DateTime.Today.AddHours(12).AddMinutes(14).AddSeconds(21);
        var endDate4 = DateTime.Today.AddHours(13).AddMinutes(23).AddSeconds(41);

        // Act
        await service.AddNewTimeAsync(1, 1, endDate1);
        await service.AddNewTimeAsync(1, 1, endDate2);
        await service.AddNewTimeAsync(1, 1, endDate3);
        await service.AddNewTimeAsync(1, 1, endDate4);
        var jobTotal = await dbContext.JobTotals.FirstOrDefaultAsync(jt => jt.JobId == 1 && jt.UserId == 1);

        // Assert
        Assert.NotNull(jobTotal);
        Assert.Equal(1, jobTotal.JobId);
        Assert.Equal(1, jobTotal.UserId);
        Assert.Equal(endDate4 - DateTime.Today.AddHours(User.DefaultDayStartTimeHours), jobTotal.TotalTime);
    }

    [Fact]
    public async Task AddNewTimeAsync_AddsMultipleNewJobTotalWithCorrectTimeMultipleUsersAndJobs()
    {
        // Arrange
        using var dbContext = CreateContext();
        var service = new TimeAggregatorService(dbContext);
        var endDate111 = DateTime.Today.AddHours(9).AddMinutes(2).AddSeconds(45);
        var endDate112 = DateTime.Today.AddHours(12).AddMinutes(11).AddSeconds(22);
        var endDate113 = DateTime.Today.AddHours(14).AddMinutes(14).AddSeconds(21);
        var endDate121 = DateTime.Today.AddHours(15).AddMinutes(34).AddSeconds(42);
        var endDate122 = DateTime.Today.AddHours(16).AddMinutes(54).AddSeconds(1);

        var endDate211 = DateTime.Today.AddHours(12).AddMinutes(14).AddSeconds(21);
        var endDate212 = DateTime.Today.AddHours(13).AddMinutes(23).AddSeconds(41);
        var endDate221 = DateTime.Today.AddHours(14).AddMinutes(14).AddSeconds(21);
        var endDate222 = DateTime.Today.AddHours(16).AddMinutes(23).AddSeconds(49);
        var endDate223 = DateTime.Today.AddHours(18).AddMinutes(15).AddSeconds(18);

        // Act
        await service.AddNewTimeAsync(1, 1, endDate111);
        await service.AddNewTimeAsync(1, 1, endDate112);
        await service.AddNewTimeAsync(1, 1, endDate113);
        await service.AddNewTimeAsync(1, 2, endDate121);
        await service.AddNewTimeAsync(1, 2, endDate122);

        await service.AddNewTimeAsync(2, 1, endDate211);
        await service.AddNewTimeAsync(2, 1, endDate212);
        await service.AddNewTimeAsync(2, 2, endDate221);
        await service.AddNewTimeAsync(2, 2, endDate222);
        await service.AddNewTimeAsync(2, 2, endDate223);

        var jobTotal11 = await dbContext.JobTotals.FirstOrDefaultAsync(jt => jt.JobId == 1 && jt.UserId == 1);
        var jobTotal12 = await dbContext.JobTotals.FirstOrDefaultAsync(jt => jt.JobId == 2 && jt.UserId == 1);
        var jobTotal21 = await dbContext.JobTotals.FirstOrDefaultAsync(jt => jt.JobId == 1 && jt.UserId == 2);
        var jobTotal22 = await dbContext.JobTotals.FirstOrDefaultAsync(jt => jt.JobId == 2 && jt.UserId == 2);

        // Assert
        Assert.NotNull(jobTotal11);
        Assert.NotNull(jobTotal12);
        Assert.NotNull(jobTotal21);
        Assert.NotNull(jobTotal22);
        Assert.Equal(endDate113 - DateTime.Today.AddHours(User.DefaultDayStartTimeHours), jobTotal11.TotalTime);
        Assert.Equal(endDate122 - endDate113, jobTotal12.TotalTime);
        Assert.Equal(endDate212 - DateTime.Today.AddHours(User.DefaultDayStartTimeHours), jobTotal21.TotalTime);
        Assert.Equal(endDate223 - endDate212, jobTotal22.TotalTime);
    }

    [Fact]
    public async Task AddNewTimeAsync_AddsMultipleNewJobTotalWithCorrectTimeMultipleInterlacedJobs()
    {
        // Arrange
        using var dbContext = CreateContext();
        var service = new TimeAggregatorService(dbContext);
        var endDate1 = DateTime.Today.AddHours(9).AddMinutes(20).AddSeconds(30);
        var endDate2 = DateTime.Today.AddHours(10).AddMinutes(2).AddSeconds(45);
        var endDate3 = DateTime.Today.AddHours(14).AddMinutes(13).AddSeconds(7);
        var endDate4 = DateTime.Today.AddHours(15).AddMinutes(45).AddSeconds(0);
        var endDate5 = DateTime.Today.AddHours(17).AddMinutes(8).AddSeconds(14);

        var expectedJob1 = (endDate5 - endDate4) + (endDate3 - endDate2) + (endDate1 - DateTime.Today.AddHours(User.DefaultDayStartTimeHours));
        var expectedJob2 = (endDate4 - endDate3) + (endDate2 - endDate1);

        // Act
        await service.AddNewTimeAsync(1, 1, endDate1);
        await service.AddNewTimeAsync(1, 2, endDate2);
        await service.AddNewTimeAsync(1, 1, endDate3);
        await service.AddNewTimeAsync(1, 2, endDate4);
        await service.AddNewTimeAsync(1, 1, endDate5);

        var jobTotal1 = await dbContext.JobTotals.FirstOrDefaultAsync(jt => jt.JobId == 1 && jt.UserId == 1);
        var jobTotal2 = await dbContext.JobTotals.FirstOrDefaultAsync(jt => jt.JobId == 2 && jt.UserId == 1);

        // Assert
        Assert.NotNull(jobTotal1);
        Assert.NotNull(jobTotal2);
        Assert.Equal(expectedJob1, jobTotal1.TotalTime);
        Assert.Equal(expectedJob2, jobTotal2.TotalTime);
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
    public async Task AddNewTimeAsync_ReusesExistingUser()
    {
        // Arrange
        using var dbContext = CreateContext();
        dbContext.Users.Add(new User
        {
            Id = 1,
            Name = "Existing User",
            LastEndTime = DateTime.Today.AddHours(13).AddMinutes(23).AddSeconds(41)
        });
        await dbContext.SaveChangesAsync();
        var service = new TimeAggregatorService(dbContext);

        // Act
        await service.AddNewTimeAsync(1, 1, DateTime.UtcNow);
        var user = await dbContext.Users.Where(u => u.Id == 1).ToListAsync();

        // Assert
        Assert.Single(user);
        Assert.Equal(1, user[0].Id);
        Assert.Equal("Existing User", user[0].Name);
    }

    [Fact]
    public async Task AddNewTimeAsync_ReusesExistingJobTotal()
    {
        // Arrange
        using var dbContext = CreateContext();
        dbContext.Jobs.Add(new Job
        {
            Id = 1,
            Description = "Existing Job"
        });
        dbContext.Users.Add(new User
        {
            Id = 1,
            Name = "Existing User",
            LastEndTime = DateTime.Today.AddHours(13).AddMinutes(23).AddSeconds(41)
        });
        dbContext.JobTotals.Add(new JobTotal
        {
            JobId = 1,
            UserId = 1,
            TotalTime = TimeSpan.FromHours(5)
        });
        await dbContext.SaveChangesAsync();
        var service = new TimeAggregatorService(dbContext);

        // Act
        await service.AddNewTimeAsync(1, 1, DateTime.UtcNow);
        var jobTotal = await dbContext.JobTotals.Where(jt => jt.JobId == 1 && jt.UserId == 1).ToListAsync();

        // Assert
        Assert.Single(jobTotal);
        Assert.Equal(1, jobTotal[0].JobId);
        Assert.Equal(1, jobTotal[0].UserId);
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
