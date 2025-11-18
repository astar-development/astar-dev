using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<AStar_Web_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<AStar_Dev_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
