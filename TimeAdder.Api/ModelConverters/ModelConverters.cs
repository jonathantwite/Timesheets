using TimeAdder.Api.Contracts.Messages;
using TimeAdder.Api.Contracts.Requests;

namespace TimeAdder.Api.ModelConverters;

public static class ModelConverters
{
    public static RecordTimeMessage AsRecordTimeMessage(this TimeRequest request, int userId) =>
        new RecordTimeMessage(userId, request.JobId, request.Date.ToDateTime(request.Time));
}
