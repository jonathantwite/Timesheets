var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var rabbitmq = builder.AddRabbitMQ("messaging").WithManagementPlugin();

//var apiService = builder.AddProject<Projects.Timesheets_ApiService>("apiservice");
//
//builder.AddProject<Projects.Timesheets_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);

builder.AddProject<Projects.TimeRecorder>("timerecorder")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.TimeAdder_Api>("timeadder-api")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.Build().Run();
