using AggregatedTimeDatabase.Entities;

namespace TimeAggregator.Models;
public class UserTests
{
    //Test User.Create
    [Fact]
    public void Create_ShouldInitializeUserWithGivenId()
    {
        // Arrange
        int userId = 1;

        // Act
        var user = User.Create(userId);

        // Assert
        Assert.Equal(userId, user.Id);
        Assert.Equal("", user.Name);
        Assert.Equal(DateTime.Today.AddHours(User.DefaultDayStartTimeHours), user.LastEndTime);
    }

    //Test User.TotalTimeRecorded
    [Fact]
    public void TotalTimeRecorded_ShouldReturnCorrectTotalTime()
    {
        // Arrange
        var user = new User
        {
            JobTotals =
            [
                new JobTotal { TotalTime = TimeSpan.FromHours(2) },
                new JobTotal { TotalTime = TimeSpan.FromHours(3) }
            ]
        };

        // Act
        var totalTime = user.TotalTimeRecorded();

        // Assert
        Assert.Equal(TimeSpan.FromHours(5), totalTime);
    }

    [Fact]
    public void TotalTimeRecorded_ShouldReturnZero_WhenNoJobTotals()
    {
        // Arrange
        var user = User.Create(1);

        // Act
        var totalTime = user.TotalTimeRecorded();

        // Assert
        Assert.Equal(TimeSpan.Zero, totalTime);
    }

    //Test User.JobDescriptions
    [Fact]
    public void JobDescriptions_ShouldReturnDistinctJobDescriptions()
    {
        // Arrange
        var user = new User
        {
            JobTotals =
            [
                new JobTotal { Job = new Job { Description = "Job1" } },
                new JobTotal { Job = new Job { Description = "Job2" } },
                new JobTotal { Job = new Job { Description = "Job1" } }
            ]
        };

        // Act
        var jobDescriptions = user.JobDescriptions();

        // Assert
        Assert.Equal(["Job1", "Job2"], jobDescriptions);
    }

    [Fact]
    public void JobDescriptions_ShouldReturnEmpty_WhenNoJobTotals()
    {
        // Arrange
        var user = User.Create(1);

        // Act
        var jobDescriptions = user.JobDescriptions();

        // Assert
        Assert.Empty(jobDescriptions);
    }

    [Fact]
    public void JobDescriptions_ShouldIgnoreEmptyDescriptions()
    {
        // Arrange
        var user = new User
        {
            JobTotals =
            [
                new JobTotal { Job = new Job { Description = "Job1" } },
                new JobTotal { Job = new Job { Description = "" } },
                new JobTotal { Job = new Job { Description = "Job2" } }
            ]
        };

        // Act
        var jobDescriptions = user.JobDescriptions();

        // Assert
        Assert.Equal(["Job1", "Job2"], jobDescriptions);
    }
}
