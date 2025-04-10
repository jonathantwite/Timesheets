namespace RawTimeEntriesDatabase.Entities;

public class TimeEntry
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int JobId { get; set; }
    public DateTime EndTime { get; set; }
}
