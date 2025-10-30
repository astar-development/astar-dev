using AStar.Dev.AppHost.Configurations;
using JetBrains.Annotations;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

builder.AddApplicationProjects();

builder.Build().Run();

namespace AStar.Dev.AppHost
{
    [UsedImplicitly]
    public class AppHost;
}
