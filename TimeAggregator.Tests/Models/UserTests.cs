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
}
