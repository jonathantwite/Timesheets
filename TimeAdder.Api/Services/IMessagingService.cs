namespace TimeAdder.Api.Services;

public interface IMessagingService
{
    void SendMessage<T>(string queue, T message);
}