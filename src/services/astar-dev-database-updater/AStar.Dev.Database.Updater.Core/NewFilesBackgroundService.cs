using AStar.Dev.Functional.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AStar.Dev.Database.Updater.Core;

/// <summary>
///     The <see cref="NewFilesBackgroundService" /> class
/// </summary>
/// <param name="addNewFilesService">The <see cref="AddNewFilesService" /> required by the AddNewFilesBackgroundService</param>
/// <param name="timeDelay">An instance of <see cref="TimeDelay" /> to control the start time of the process</param>
/// <param name="config">An instance of the <see cref="DatabaseUpdaterConfiguration" /> options used to configure the addition of the new files</param>
/// <param name="logger">An instance of the <see cref="ILogger" /> to log status / errors</param>
/// <param name="taskDelayer">An optional interface to handle Task.Delay for better testability</param>
public class NewFilesBackgroundService(
    AddNewFilesService                     addNewFilesService,
    TimeDelay                              timeDelay,
    IOptions<DatabaseUpdaterConfiguration> config,
    ILogger<NewFilesBackgroundService>     logger,
    ITaskDelayer?                          taskDelayer = null)
    : BackgroundService
{
    private readonly ITaskDelayer _taskDelayer = taskDelayer ?? new DefaultTaskDelayer();

    /// <summary>
    ///     The StartAsync method is called by the runtime and will update the database with any new files
    /// </summary>
    /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.CompletedTask;
        var startAtTime = config.Value.NewFilesScheduledTime;

        var delay = config.Value.HonourFirstDelay
                        ? timeDelay.CalculateDelayToNextRun(startAtTime)
                        : new Result<TimeSpan, ErrorResponse>.Ok(TimeSpan.Zero);

        while(!stoppingToken.IsCancellationRequested)
        {
            var runAddNewFilesResult = await delay.Tap(LogMessage)
                                                  .BindAsync(async timeSpan => await AwaitDelay(timeSpan, stoppingToken))
                                                  .BindAsync(async timeSpan => await addNewFilesService.StartAsync(timeSpan, stoppingToken));

            _ = await runAddNewFilesResult.Match<Result<TimeSpan, ErrorResponse>>(_ => timeDelay.CalculateDelayToNextRun(startAtTime),
                                                                                  _ => new Result<TimeSpan, ErrorResponse>.Error(new("Error.")))
                                          .BindAsync(async timeSpan => await AwaitDelay(timeSpan, stoppingToken))
                                          .TapErrorAsync(async f => await LogErrorMessage(stoppingToken, f));
        }
    }

    private async Task LogErrorMessage(CancellationToken stoppingToken, ErrorResponse errorResponse)
    {
        logger.LogError("Error: {ErrorMessage}", errorResponse.Message);
        await Task.Delay(1, stoppingToken);
    }

    private async Task<Result<TimeSpan, ErrorResponse>> AwaitDelay(TimeSpan timeSpan, CancellationToken stoppingToken)
    {
        await _taskDelayer.Delay(timeSpan, stoppingToken);

        return new Result<TimeSpan, ErrorResponse>.Ok(timeSpan);
    }

    private void LogMessage(TimeSpan delay)
        => logger.LogInformation("Waiting for: {DelayToNextRun} hours before updating the full database again.", delay.TotalHours);
}
