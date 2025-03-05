namespace TimeAdder.Api.Contracts.Requests;

public record TimeRequest (int JobId, TimeOnly Time) { }
