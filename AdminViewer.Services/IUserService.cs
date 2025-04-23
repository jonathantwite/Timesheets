using AdminViewer.Services.DTOs;

namespace AdminViewer.Services;
public interface IUserService
{
    Task AddUser(int userId, string name);
    Task<IEnumerable<MissingUser>> GetMissingUsersAsync();
}