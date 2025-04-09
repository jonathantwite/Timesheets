var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var rabbitmq = builder.AddRabbitMQ("messaging").WithManagementPlugin();

var dbServer = builder.AddSqlServer("sqlDbServer").WithLifetime(ContainerLifetime.Persistent);
var rawTimeEntriesDb = dbServer.AddDatabase("RawTimeEntriesDb");
var aggregatedTimeDb = dbServer.AddDatabase("AggregatedTimeDb");

//var apiService = builder.AddProject<Projects.Timesheets_ApiService>("apiservice");
//
//builder.AddProject<Projects.Timesheets_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);

builder.AddProject<Projects.TimeRecorder>("timerecorder")
    .WithReplicas(2)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(dbServer)
    .WaitFor(dbServer);

builder.AddProject<Projects.TimeAggregator>("timeaggregator")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(aggregatedTimeDb)
    .WaitFor(aggregatedTimeDb);

builder.AddProject<Projects.TimeAdder_Api>("timeadder-api")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.AddAzureFunctionsProject<Projects.NightlyCleanup>("nightlycleanup")
    .WithReference(dbServer)
    .WaitFor(dbServer);

builder.AddProject<Projects.Database_MigrationService>("database-migrationservice")
    .WithReference(rawTimeEntriesDb)
    .WaitFor(rawTimeEntriesDb)
    .WithReference(aggregatedTimeDb)
    .WaitFor(aggregatedTimeDb);

builder.Build().Run();
