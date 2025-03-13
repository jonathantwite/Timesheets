using AggregatedTimeDatabase;
using AggregatedTimeDatabase.Entities;
using Microsoft.EntityFrameworkCore;

namespace TimeAggregator.Services;

public class TimeAggregatorService(AggregatedTimeContext dbContext) : ITimeAggregatorService
{
    private readonly AggregatedTimeContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task AddNewTime(int userId, int jobId, TimeSpan totalTime)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);

        if (job == null)
        {
            job = new Job
            {
                Id = jobId,
                Description = ""
            };
            _dbContext.Jobs.Add(job);
        }

        if (user == null)
        {
            user = new User
            {
                Id = userId,
                Name = ""
            };
            _dbContext.Users.Add(user);
        }

        var jt = job.JobTotals.SingleOrDefault(jt => jt.UserId == userId);
        if (jt == null)
        {
            jt = new JobTotal
            {
                JobId = jobId,
                UserId = userId,
                TotalTime = new TimeSpan(0, 0, 0)
            };
            job.JobTotals.Add(jt);
        }

        jt.TotalTime.Add(totalTime);

        await _dbContext.SaveChangesAsync();

    }

    public Task CleanUp()
    {

    }
}
