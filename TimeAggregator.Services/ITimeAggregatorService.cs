
namespace TimeAggregator.Services;

public interface ITimeAggregatorService
{
    Task AddNewTime(int userId, int jobId, TimeSpan totalTime);
    Task CleanUp();
}