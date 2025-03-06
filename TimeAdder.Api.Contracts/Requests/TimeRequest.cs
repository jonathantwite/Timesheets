namespace TimeAdder.Api.Contracts.Requests;

public record TimeRequest (int JobId, DateOnly Date, TimeOnly Time) { }
