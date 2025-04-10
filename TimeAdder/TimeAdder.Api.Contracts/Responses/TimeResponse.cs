namespace TimeAdder.Api.Contracts.Responses;

public record TimeResponse(string JobDescription, TimeOnly StartTime, TimeOnly EndTime);
