namespace AggregatedTimeDatabase.Entities;

public class Job
{
    public int Id { get; set; }
    public string? Description { get; set; }

    public ICollection<JobTotal> JobTotals = [];

    public static Job Create(int jobId) => new() { Id = jobId, Description = "" };
}
