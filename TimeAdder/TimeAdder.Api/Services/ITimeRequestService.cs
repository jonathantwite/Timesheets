using TimeAdder.Api.Contracts.Requests;

namespace TimeAdder.Api.Services;
public interface ITimeRequestService
{
    void ProcessNewTimeRequest(int userId, TimeRequest request);
}