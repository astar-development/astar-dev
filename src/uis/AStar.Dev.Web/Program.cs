using AStar.Dev.Web;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();

WebApplication app = builder.RemoveServerHeaderAndBuild();

app.UseApplicationServices();

await app.RunAsync();
