using AggregatedTimeDatabase;
using Microsoft.EntityFrameworkCore;
using RawTimeEntriesDatabase;
using System.Diagnostics;

namespace Database.MigrationService;

public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime, ILogger<Worker> logger) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly IHostApplicationLifetime _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));

    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating databases", ActivityKind.Client);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var atDbContext = scope.ServiceProvider.GetRequiredService<AggregatedTimeContext>();
            var rtDbContext = scope.ServiceProvider.GetRequiredService<RawTimeEntriesContext>();

            await Task.WhenAll(
                MigrateDatabaseAsync<AggregatedTimeContext>(atDbContext, cancellationToken),
                MigrateDatabaseAsync<RawTimeEntriesContext>(rtDbContext, cancellationToken)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the databases.");
            _hostApplicationLifetime.StopApplication();

            while (!cancellationToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, cancellationToken);
            }
        }
        finally
        {
            _hostApplicationLifetime.StopApplication();
        }
    }

    private static async Task MigrateDatabaseAsync<T>(T dbContext, CancellationToken cancellationToken) where T : DbContext
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }


}
