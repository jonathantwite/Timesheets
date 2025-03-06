using Messaging.Shared.Interfaces;

namespace TimeRecorder.Contracts.Messages;

public record RecordTimeMessage (int UserId, int JobId, DateTime EndTime): IMessage;
