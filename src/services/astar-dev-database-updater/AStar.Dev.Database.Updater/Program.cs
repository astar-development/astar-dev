using AStar.Dev.Database.Updater;
using JetBrains.Annotations;
using Serilog;
using ILogger = Serilog.ILogger;

var startTime = DateTime.Now;
var applicationName = typeof(IAssemblyMarker).Assembly.GetName().Name!;

try
{
    var builder = Host.CreateApplicationBuilder(args);
    builder.ConfigureApplicationServices();

    var app = builder.Build();

    Log.Logger = app.Services.GetRequiredService<ILogger>();
    Log.Information("Starting {AppName}", applicationName);

    await app.RunAsync();
}
catch(Exception exception)
{
    Log.Error(exception, "Fatal error occurred in {AppName}", applicationName);
}
finally
{
    Log.Information("Stopped after {ProcessingTimeMilliseconds}", DateTime.Now - startTime);
    Log.CloseAndFlush();
}

namespace AStar.Dev.Database.Updater
{
    [UsedImplicitly]
    public class Program
    {
    }
}
