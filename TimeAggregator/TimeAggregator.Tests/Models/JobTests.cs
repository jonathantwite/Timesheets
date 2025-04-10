using AggregatedTimeDatabase.Entities;

namespace TimeAggregator.Models;
public class JobTests
{
    //Test Job.Create
    [Fact]
    public void Create_ShouldInitializeJobWithGivenId()
    {
        // Arrange
        int jobId = 1;

        // Act
        var job = Job.Create(jobId);

        // Assert
        Assert.Equal(jobId, job.Id);
        Assert.Equal("", job.Description);
    }
}
