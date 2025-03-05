using TimeAdder.Api.Contracts.Requests;
using TimeAdder.Api.Contracts.Responses;
using TimeAdder.Api.Services;

namespace TimeAdder.Api.Endpoints;

public static class TimeEndpoints
{
    public static void MapTimeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("Time");

        group.MapGet("", () =>
        {
            return new List<TimeResponse>();
        });

        group.MapPost("", (TimeRequest timeRequest, IMessagingService messagingService) =>
        {
            messagingService.SendMessage("TimeAdded", timeRequest);
        });
    }
}
