using Messaging.Shared.Interfaces;

namespace TimeAdder.Api.Services;

public interface IMessagingService
{
    void SendMessage<T>(string exchange, T message) where T : IMessage;
}