using AStar.Dev.Database.Updater;
using JetBrains.Annotations;
using Serilog;
using ILogger = Serilog.ILogger;

DateTime startTime = DateTime.Now;

ILogger? logger = null;

try
{
    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
    _ = builder.ConfigureApplicationServices();

    IHost app = builder.Build();

    Log.Logger = app.Services.GetRequiredService<ILogger>();

    await app.RunAsync();
}
catch(Exception exception)
{
    logger?.Fatal(exception, "An error occurred while running the application. Message: {ErrorMessage}",
        exception.Message);
}
finally
{
    logger?.Information("Stopped after {ProcessingTimeMilliseconds}", DateTime.Now - startTime);
    await Log.CloseAndFlushAsync();
}

namespace AStar.Dev.Database.Updater
{
    [UsedImplicitly]
    public class Program
    {
    }
}
