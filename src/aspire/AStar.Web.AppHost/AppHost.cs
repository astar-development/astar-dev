using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ProjectResource> apiService = builder.AddProject<AStar_Web_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<AStar_Dev_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
