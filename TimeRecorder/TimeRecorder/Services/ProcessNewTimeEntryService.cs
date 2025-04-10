using RawTimeEntriesDatabase;
using RawTimeEntriesDatabase.Entities;
using TimeAdder.Api.Contracts.Messages;

namespace TimeRecorder.Services;

public class ProcessNewTimeEntryService(RawTimeEntriesContext dbContext) : IProcessNewTimeEntryService
{
    private readonly RawTimeEntriesContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task ProcessAsync(RecordTimeMessage message)
    {
        _dbContext.TimeEntries.Add(new TimeEntry
        {
            UserId = message.UserId,
            JobId = message.JobId,
            EndTime = message.EndTime
        });

        await _dbContext.SaveChangesAsync();
    }
}
