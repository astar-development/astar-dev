using AStar.Dev.Database.Updater;
using JetBrains.Annotations;
using Serilog;
using ILogger = Serilog.ILogger;

var startTime = DateTime.Now;

ILogger? logger = null;

try
{
    var builder = Host.CreateApplicationBuilder(args);
    builder.ConfigureApplicationServices();

    var app = builder.Build();

    Log.Logger = app.Services.GetRequiredService<ILogger>();

    // Log.Logger = new LoggerConfiguration()
    //              .MinimumLevel.Debug()
    //              .WriteTo.File("logs/log-testing.log.txt", rollingInterval: RollingInterval.Day)
    //              .CreateLogger();

    await app.RunAsync();
}
catch(Exception exception)
{
    logger?.Fatal(exception, "An error occured while running the application. Message: {ErrorMessage}",
        exception.Message);
}
finally
{
    logger?.Information("Stopped after {ProcessingTimeMilliseconds}", DateTime.Now - startTime);
    Log.CloseAndFlush();
}

namespace AStar.Dev.Database.Updater
{
    [UsedImplicitly]
    public class Program
    {
    }
}
