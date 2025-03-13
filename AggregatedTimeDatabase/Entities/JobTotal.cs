namespace AggregatedTimeDatabase.Entities;

public class JobTotal
{
    public int JobId;
    public Job Job = null!;

    public int UserId;
    public User User = null!;

    public TimeSpan TotalTime;
}
