using AStar.Dev.ServiceDefaults;
using AStar.Dev.Web.Components;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
       .AddInteractiveServerComponents();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

await app.RunAsync();

namespace AStar.Dev.Web
{
    public  class Program
    {
    }
}
