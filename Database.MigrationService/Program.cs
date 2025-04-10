using AggregatedTimeDatabase;
using Database.MigrationService;
using RawTimeEntriesDatabase;
using Timesheets.Globals;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("Database.MigrationService"/*Worker.ActivitySourceName*/));

builder.AddSqlServerDbContext<AggregatedTimeContext>(connectionName: ServiceNames.AggregatedTimeDb);
builder.AddSqlServerDbContext<RawTimeEntriesContext>(connectionName: ServiceNames.RawTimeEntriesDb);

var host = builder.Build();
host.Run();
