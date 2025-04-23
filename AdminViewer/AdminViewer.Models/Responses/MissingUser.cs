namespace AdminViewer.Models.Responses;

public record MissingUser (int Id, DateTime LastEntry, TimeSpan TotalTimeRecorded, IEnumerable<string> JobDescriptions);
