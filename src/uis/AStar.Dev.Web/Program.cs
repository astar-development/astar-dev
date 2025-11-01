using AStar.Dev.Web;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();

WebApplication app = builder.RemoveServerHeaderAndBuild();

app.UseApplicationServices();

await app.RunAsync();

#pragma warning disable S1118 // Utility classes should not have public constructors
namespace AStar.Dev.Web
{
    public class Program
    {
    }
}
#pragma warning restore S1118 // Utility classes should not have public constructors
