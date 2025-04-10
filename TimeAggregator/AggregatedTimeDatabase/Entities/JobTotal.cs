namespace AggregatedTimeDatabase.Entities;

public class JobTotal
{
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public TimeSpan TotalTime { get; set; }

    public static JobTotal Create(int jobId, int userId) => new() { JobId = jobId, UserId = userId, TotalTime = new TimeSpan(0, 0, 0) };
}
