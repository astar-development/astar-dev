using System.Diagnostics;
using AStar.Dev.Database.Updater;
using Serilog;
using ILogger = Serilog.ILogger;

var     startTime = Stopwatch.GetTimestamp();
ILogger logger    = null!;

try
{
    Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
    logger     = Log.Logger;
    var builder = Host.CreateApplicationBuilder(args);

    _ = builder.ConfigureApplicationServices();

    var host = builder.Build();

    await host.RunAsync();
}
catch(Exception ex)
{
    logger.Error(ex, "An error occured while running the application.");
}
finally
{
    var elapsedTime = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
    logger.Information("Stopped after {ProcessingTimeMilliseconds} ms", elapsedTime);
    await Log.CloseAndFlushAsync();
}
