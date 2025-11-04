using AStar.Dev.Web;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();

WebApplication app = builder.RemoveServerHeaderAndBuild();

app.UseApplicationServices();

await app.RunAsync();

#pragma warning disable S2094 // Classes should not be empty
namespace AStar.Dev.Web
{
    public class Program
    {
    }
}
#pragma warning restore S2094 // Classes should not be empty
