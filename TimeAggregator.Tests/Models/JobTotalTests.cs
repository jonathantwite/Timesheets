using AggregatedTimeDatabase.Entities;

namespace TimeAggregator.Models;

public class JobTotalTests
{
    //Test JobTotal.Create
    [Fact]
    public void Create_ShouldInitializeJobTotalWithGivenJobIdAndUserId()
    {
        // Arrange
        int jobId = 1;
        int userId = 2;

        // Act
        var jobTotal = JobTotal.Create(jobId, userId);

        // Assert
        Assert.Equal(jobId, jobTotal.JobId);
        Assert.Equal(userId, jobTotal.UserId);
        Assert.Equal(new TimeSpan(0, 0, 0), jobTotal.TotalTime);
    }
}
