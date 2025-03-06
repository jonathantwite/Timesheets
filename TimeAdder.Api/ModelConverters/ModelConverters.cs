using TimeAdder.Api.Contracts.Requests;
using TimeRecorder.Contracts.Messages;

namespace TimeAdder.Api.ModelConverters;

public static class ModelConverters
{
    public static RecordTimeMessage AsRecordTimeMessage(this TimeRequest request, int userId) =>
        new RecordTimeMessage(userId, request.JobId, request.Date.ToDateTime(request.Time));
}
