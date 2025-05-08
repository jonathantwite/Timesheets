using AggregatedTimeDatabase;
using AggregatedTimeDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Timesheets.Globals;

namespace Timesheets.Tests.ApiEndToEndTests;
public class TimeAdderTests
{
    [Test]
    public async Task PostTime_AddsNewRawTimeEntry()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Timesheets_AppHost>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
        await using var app = await appHost.BuildAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync();

        var httpClient = app.CreateHttpClient(ServiceNames.TimeAdderApi);
        await Task.WhenAll(
            resourceNotificationService.WaitForResourceAsync(ServiceNames.RabbitMQ, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(300)),
            resourceNotificationService.WaitForResourceAsync(ServiceNames.SqlDbServer, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(300)),
            resourceNotificationService.WaitForResourceAsync(ServiceNames.AggregatedTimeDb, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(300)),
            resourceNotificationService.WaitForResourceAsync(ServiceNames.DatabaseMigrationService, KnownResourceStates.Finished).WaitAsync(TimeSpan.FromSeconds(300)),
            resourceNotificationService.WaitForResourceAsync(ServiceNames.TimeAdderApi, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(300)),
            resourceNotificationService.WaitForResourceAsync(ServiceNames.TimeAggregator, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(300)));
        await Task.Delay(180000);

        var connectionString = await app.GetConnectionStringAsync(ServiceNames.AggregatedTimeDb);
        var contextOptions = new DbContextOptionsBuilder<AggregatedTimeContext>()
            .UseSqlServer(connectionString)
            .Options;

        using var context = new AggregatedTimeContext(contextOptions);

        // Act
        var response = await httpClient.PostAsJsonAsync("/Time", new { JobId = 1, Date = DateOnly.FromDateTime(DateTime.Today), Time = new TimeOnly(User.DefaultDayStartTimeHours + 5, 0) });

        await Task.Delay(2000);

        var actual = await context.JobTotals
            .Where(jt => jt.UserId == 1 && jt.JobId == 1)
            .ToListAsync();


        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual[0].TotalTime, Is.EqualTo(TimeSpan.FromHours(5)));
        });
    }
}
