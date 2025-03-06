using Messaging.Shared.Constants;
using TimeAdder.Api.Contracts.Requests;
using TimeAdder.Api.ModelConverters;
using TimeRecorder.Contracts.Messages;

namespace TimeAdder.Api.Services;

public class TimeRequestService(IMessagingService messagingService) : ITimeRequestService
{
    private readonly IMessagingService _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));


    public void ProcessNewTimeRequest(int userId, TimeRequest request)
    {
        _messagingService.SendMessage<RecordTimeMessage>(MessagingConstants.TimeRecorderQueueName, request.AsRecordTimeMessage(userId));
    }
}
