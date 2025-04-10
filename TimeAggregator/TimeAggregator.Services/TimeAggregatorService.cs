using AggregatedTimeDatabase;
using AggregatedTimeDatabase.Entities;
using Microsoft.EntityFrameworkCore;

namespace TimeAggregator.Services;

public class TimeAggregatorService(AggregatedTimeContext dbContext) : ITimeAggregatorService
{
    private readonly AggregatedTimeContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task AddNewTimeAsync(int userId, int jobId, DateTime endTime)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);

        if (job == null)
        {
            job = Job.Create(jobId);
            _dbContext.Jobs.Add(job);
        }

        if (user == null)
        {
            user = User.Create(userId);
            _dbContext.Users.Add(user);
        }

        var jt = job.JobTotals.SingleOrDefault(jt => jt.UserId == userId && jt.JobId == jobId);
        if (jt == null)
        {
            jt = JobTotal.Create(jobId, userId);
            job.JobTotals.Add(jt);
        }

        jt.TotalTime = jt.TotalTime.Add(endTime - user.LastEndTime);
        _dbContext.JobTotals.Add(jt);

        user.LastEndTime = endTime;

        await _dbContext.SaveChangesAsync();
    }

    public async Task CleanUpAsync()
    {
        //Update overtime table
        _dbContext.OvertimeRecords.AddRange(Overtime.Create(_dbContext.Users.ToList()));

        //Reset users last time
        _dbContext.Users.ToList().ForEach(u => u.LastEndTime = DateTime.MinValue);

        await _dbContext.SaveChangesAsync();
    }
}
