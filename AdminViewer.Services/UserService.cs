using AdminViewer.Models.Requests;
using AdminViewer.Models.Responses;
using AggregatedTimeDatabase;
using AggregatedTimeDatabase.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminViewer.Services;
public class UserService(AggregatedTimeContext dbContext) : IUserService
{
    private readonly AggregatedTimeContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<IEnumerable<MissingUser>> GetMissingUsersAsync() =>
        await _dbContext.Users
            .Include(u => u.JobTotals)
            .ThenInclude(jt => jt.Job)
            .Where(u => string.IsNullOrEmpty(u.Name))
            .Select(u => new MissingUser(
                u.Id,
                u.LastEndTime,
                u.TotalTimeRecorded(),
                u.JobDescriptions()))
            .AsNoTracking()
            .ToListAsync();

    public async Task AddUser(AddUserRequest userRequest)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userRequest.UserId);

        if (user == null)
        {
            user = User.Create(userRequest.UserId);
            _dbContext.Users.Add(user);
        }

        user.Name = userRequest.Name;
        await _dbContext.SaveChangesAsync();
    }
}
