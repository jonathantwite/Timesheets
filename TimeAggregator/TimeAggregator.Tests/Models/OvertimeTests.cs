using AggregatedTimeDatabase.Entities;

namespace TimeAggregator.Models;
public class OvertimeTests
{
    [Fact]
    public void Create_ShouldReturnNull_WhenTotalTimeIsLessThanMaxTimeBeforeOvertime()
    {
        // Arrange
        int userId = 1;
        DateOnly date = new DateOnly(2023, 10, 1);
        TimeSpan totalTime = new TimeSpan(7, 0, 0); // Less than MaxTimeBeforeOvertime

        // Act
        var result = Overtime.Create(userId, date, totalTime);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Create_ShouldReturnOvertime_WhenTotalTimeIsGreaterThanMaxTimeBeforeOvertime()
    {
        // Arrange
        int userId = 1;
        DateOnly date = new DateOnly(2023, 10, 1);
        TimeSpan totalTime = new TimeSpan(8, 0, 0); // Greater than MaxTimeBeforeOvertime

        // Act
        var result = Overtime.Create(userId, date, totalTime);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(date, result.Date);
        Assert.Equal(new TimeSpan(0, 30, 0), result.OvertimeTime); // Overtime time should be totalTime - MaxTimeBeforeOvertime
    }
}
