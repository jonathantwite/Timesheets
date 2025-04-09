using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TestingUtilities;

public abstract class DbMockedTestFixture<T> : IDisposable where T : DbContext
{
    private protected SqliteConnection _connection;
    private protected DbContextOptions<T> _contextOptions;

    public DbMockedTestFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _contextOptions = new DbContextOptionsBuilder<T>()
            .UseSqlite(_connection)
            .Options;
        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    protected T CreateContext() => (T)Activator.CreateInstance(typeof(T), _contextOptions);

    public void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}