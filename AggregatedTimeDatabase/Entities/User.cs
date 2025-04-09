namespace AggregatedTimeDatabase.Entities;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public DateTime LastEndTime { get; set; }

    public ICollection<JobTotal> JobTotals = [];
    public ICollection<Overtime> OvertimeRecords = [];

    public static User Create(int userId) => new() { Id = userId, Name = "", LastEndTime = DateTime.Today.AddHours(DefaultDayStartTimeHours) };

    public static int DefaultDayStartTimeHours = 9;
}
