
namespace TimeAggregator.Services;

public interface ITimeAggregatorService
{
    Task AddNewTimeAsync(int userId, int jobId, DateTime endTime);
    Task CleanUpAsync();
}