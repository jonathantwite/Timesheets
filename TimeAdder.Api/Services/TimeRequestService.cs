using Messaging.Shared.Constants;
using TimeAdder.Api.Contracts.Messages;
using TimeAdder.Api.Contracts.Requests;
using TimeAdder.Api.ModelConverters;

namespace TimeAdder.Api.Services;

public class TimeRequestService(IMessagingService messagingService) : ITimeRequestService
{
    private readonly IMessagingService _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));


    public void ProcessNewTimeRequest(int userId, TimeRequest request)
    {
        _messagingService.SendMessage<RecordTimeMessage>(MessagingConstants.NewTimeRecordedExchange, request.AsRecordTimeMessage(userId));
    }
}
