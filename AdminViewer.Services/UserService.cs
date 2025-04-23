using AdminViewer.Services.DTOs;
using AggregatedTimeDatabase;
using AggregatedTimeDatabase.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminViewer.Services;
public class UserService(AggregatedTimeContext dbContext) : IUserService
{
    private readonly AggregatedTimeContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<IEnumerable<MissingUser>> GetMissingUsersAsync() =>
        await _dbContext.Users
            .Where(u => string.IsNullOrEmpty(u.Name))
            .Select(u => new MissingUser(
                u.Id,
                u.LastEndTime,
                u.TotalTimeRecorded(),
                u.JobDescriptions()))
            .AsNoTracking()
            .ToListAsync();

    public async Task AddUser(int userId, string name)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            user = User.Create(userId);
            _dbContext.Users.Add(user);
        }

        user.Name = name;
        await _dbContext.SaveChangesAsync();
    }
}
