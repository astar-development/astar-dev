using AStar.Dev.Database.Updater;
using Serilog;
using ILogger = Serilog.ILogger;

var startTime = DateTime.Now;

ILogger? logger = null;

try
{
    var builder = Host.CreateApplicationBuilder(args)
                      .ConfigureApplicationServices();

    var app = builder.Build();

    logger     = app.Services.GetRequiredService<ILogger>();
    Log.Logger = logger;

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

public partial class Program
{
}
