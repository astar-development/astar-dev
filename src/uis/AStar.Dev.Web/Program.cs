using AStar.Dev.Web;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();

var app = builder.RemoveServerHeaderAndBuild();

app.UseApplicationServices();

await app.RunAsync();
