namespace AggregatedTimeDatabase.Entities;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public DateTime LastEndTime { get; set; }

    public ICollection<JobTotal> JobTotals { get; set; } = [];
    public ICollection<Overtime> OvertimeRecords { get; set; } = [];

    public static User Create(int userId) => new() { Id = userId, Name = "", LastEndTime = DateTime.Today.AddHours(DefaultDayStartTimeHours) };

    public const int DefaultDayStartTimeHours = 9;

    public TimeSpan TotalTimeRecorded() => TimeSpan.FromMinutes(JobTotals.Select(jt => jt.TotalTime).Sum(ts => ts.TotalMinutes));

    public IEnumerable<string> JobDescriptions() => JobTotals
        .Select(jt => jt.Job.Description)
        .Where(j => !string.IsNullOrEmpty(j))
        .Distinct()
        .ToList();
}
