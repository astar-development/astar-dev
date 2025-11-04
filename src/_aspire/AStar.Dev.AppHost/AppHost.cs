using AStar.Dev.AppHost.Configurations;
using JetBrains.Annotations;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

builder.AddApplicationProjects();

await builder.Build().RunAsync();

namespace AStar.Dev.AppHost
{
    [UsedImplicitly]
    public class AppHost;
}
