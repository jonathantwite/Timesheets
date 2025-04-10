using Messaging.Shared.Interfaces;

namespace TimeAdder.Api.Contracts.Messages;

public record RecordTimeMessage (int UserId, int JobId, DateTime EndTime): IMessage;
