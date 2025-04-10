namespace AggregatedTimeDatabase.Entities;
public class Overtime
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public TimeSpan OvertimeTime { get; set; }

    public static readonly TimeSpan MaxTimeBeforeOvertime = new TimeSpan(7, 30, 0);

    public static Overtime? Create(int userId, DateOnly date, TimeSpan totalTime)
    {
        if (totalTime < MaxTimeBeforeOvertime)
        {
            return null;
        }

        var overtime = totalTime - MaxTimeBeforeOvertime;

        return new()
        {
            UserId = userId,
            Date = date,
            OvertimeTime = overtime
        };
    }

    public static IEnumerable<Overtime> Create(IEnumerable<User> users) => users
        .Select(u => Overtime.Create(u.Id, DateOnly.FromDateTime(u.LastEndTime.Date), u.LastEndTime - u.LastEndTime.Date.AddHours(User.DefaultDayStartTimeHours)))
        .Where(o => o != null)
        .Cast<Overtime>();
}
