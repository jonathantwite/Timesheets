using AdminViewer.Models.Requests;
using AdminViewer.Models.Responses;

namespace AdminViewer.Services;
public interface IUserService
{
    Task AddUser(AddUserRequest userRequest);
    Task<IEnumerable<MissingUser>> GetMissingUsersAsync();
}