using AggregatedTimeDatabase;
using Database.MigrationService;
using RawTimeEntriesDatabase;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("Database.MigrationService"/*Worker.ActivitySourceName*/));

builder.AddSqlServerDbContext<AggregatedTimeContext>(connectionName: "AggregatedTimeDb");
builder.AddSqlServerDbContext<RawTimeEntriesContext>(connectionName: "RawTimeEntriesDb");

var host = builder.Build();
host.Run();
