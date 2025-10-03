using AStar.Dev.Database.Updater;
using JetBrains.Annotations;
using Serilog;
using ILogger = Serilog.ILogger;

var startTime = DateTime.Now;

ILogger? logger = null;

try
{
    var builder = Host.CreateApplicationBuilder(args)
                      .ConfigureApplicationServices();

    var app = builder.Build();

    Log.Logger = app.Services.GetRequiredService<ILogger>();

    app.Run();
}
catch(Exception exception)
{
    logger?.Fatal(exception, "An error occured while running the application. Message: {ErrorMessage}", exception.Message);
}
finally
{
    logger?.Information("Stopped after {ProcessingTimeMilliseconds}", DateTime.Now - startTime);
    Log.CloseAndFlush();
}

[UsedImplicitly]
public partial class Program
{
}
