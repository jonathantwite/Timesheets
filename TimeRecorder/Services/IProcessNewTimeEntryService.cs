using TimeAdder.Api.Contracts.Messages;

namespace TimeRecorder.Services;
public interface IProcessNewTimeEntryService
{
    Task ProcessAsync(RecordTimeMessage message);
}