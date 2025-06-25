using AStar.Dev.Aspire.Common;
using Projects;

namespace AStar.Dev.AppHost;

public static class DistributedApplicationBuilderExtensions
{
    private const string HealthEndpoint = "/health";

    public static
        void AddApplicationProjects(this IDistributedApplicationBuilder distributedApplicationBuilder)
    {
        var usageApi = AddUsageApi(distributedApplicationBuilder);
        var imagesApi = AddImagesApi(distributedApplicationBuilder);
        var filesApi = AddFilesApi(distributedApplicationBuilder);
        var adminApi = AddAdminApi(distributedApplicationBuilder);

        AddUi(distributedApplicationBuilder, adminApi, filesApi, imagesApi, usageApi);
    }

    private static void AddUi(IDistributedApplicationBuilder           distributedApplicationBuilder,
                              IResourceBuilder<ProjectResource>        adminApi,
                              IResourceBuilder<ProjectResource>        filesApi,
                              IResourceBuilder<ProjectResource>        imagesApi,
                              IResourceBuilder<ProjectResource>        usageApi) =>
        distributedApplicationBuilder.AddProject<AStar_Dev_Web>(AspireConstants.Ui)
                                     .WithExternalHttpEndpoints()
                                     .WithHttpHealthCheck(HealthEndpoint)
                                     .WithReference(adminApi)
                                     .WaitFor(adminApi)
                                     .WithReference(filesApi)
                                     .WaitFor(filesApi)
                                     .WithReference(imagesApi)
                                     .WaitFor(imagesApi)
                                     .WithReference(usageApi)
                                     .WaitFor(usageApi);

    private static IResourceBuilder<ProjectResource> AddUsageApi(IDistributedApplicationBuilder distributedApplicationBuilder) =>
        distributedApplicationBuilder.AddProject<AStar_Dev_Usage_Logger>(AspireConstants.Apis.UsageApi)
                                     .WithHttpHealthCheck(HealthEndpoint);

    private static IResourceBuilder<ProjectResource> AddImagesApi(IDistributedApplicationBuilder distributedApplicationBuilder) =>
        distributedApplicationBuilder.AddProject<AStar_Dev_Images_Api>(AspireConstants.Apis.ImagesApi)
                                     .WithHttpHealthCheck(HealthEndpoint);

    private static IResourceBuilder<ProjectResource> AddFilesApi(IDistributedApplicationBuilder              distributedApplicationBuilder) =>
        distributedApplicationBuilder.AddProject<AStar_Dev_Files_Api>(AspireConstants.Apis.FilesApi)
                                     .WithHttpHealthCheck(HealthEndpoint);

    private static IResourceBuilder<ProjectResource> AddAdminApi(IDistributedApplicationBuilder              distributedApplicationBuilder) =>
        distributedApplicationBuilder.AddProject<AStar_Dev_Admin_Api>(AspireConstants.Apis.AdminApi)
                                     .WithHttpHealthCheck(HealthEndpoint);

}
