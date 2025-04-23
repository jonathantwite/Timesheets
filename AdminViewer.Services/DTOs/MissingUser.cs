namespace AdminViewer.Services.DTOs;

public record MissingUser (int Id, DateTime LastEntry, TimeSpan TotalTimeRecorded, IEnumerable<string> JobDescriptions);
