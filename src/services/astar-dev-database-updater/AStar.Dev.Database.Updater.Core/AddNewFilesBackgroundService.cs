using AStar.Dev.Functional.Extensions;
using AStar.Dev.Utilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AStar.Dev.Database.Updater.Core;

/// <summary>
///     The <see cref="AddNewFilesBackgroundService" /> class
/// </summary>
/// <param name="addNewFilesService">The <see cref="AddNewFilesService" /> required by the AddNewFilesBackgroundService</param>
/// <param name="timeDelay">An instance of <see cref="TimeDelay" /> to control the start time of the process</param>
/// <param name="config">An instance of the <see cref="DatabaseUpdaterConfiguration" /> options used to configure the addition of the new files</param>
/// <param name="logger">An instance of the <see cref="ILogger" /> to log status / errors</param>
public class AddNewFilesBackgroundService(AddNewFilesService addNewFilesService, TimeDelay timeDelay, IOptions<DatabaseUpdaterConfiguration> config, ILogger<AddNewFilesBackgroundService> logger)
    : BackgroundService
{
    /// <summary>
    ///     The StartAsync method is called by the runtime and will update the database with any new files
    /// </summary>
    /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.CompletedTask;
        var startAtTime = config.Value.NewFilesScheduledTime;
        logger.ToJson();

        while(!stoppingToken.IsCancellationRequested)
        {
            var delay = config.Value.HonourFirstDelay
                            ? timeDelay.CalculateDelayToNextRun(startAtTime)
                            : new Result<TimeSpan, ErrorResponse>.Ok(TimeSpan.Zero);

            // var test = Result<>.Create(timeDelay.CalculateDelayToNextRun(startAtTime));

            // delay.Bind(delayToNextRun => logger.LogInformation("Waiting for: {DelayToNextRun} hours before updating the full database again.", delayToNextRun));
            //
            // var result = await delay.MatchAsync(
            //                                                  name => Task.FromResult($"Hello"),
            //                                                  () => Task.FromResult("User not found")
            //                                                 );

            await Task.Delay(delay.Match(x => x, _ => TimeSpan.Zero), stoppingToken);

            await addNewFilesService.StartAsync(stoppingToken);

            //.Bind(delay=> logger.LogInformation("Adding new files..."));
            //.Bind(delayToNextRun => logger.LogInformation("Waiting for: {DelayToNextRun} hours before updating the full database again.", delayToNextRun))
            //.OnSuccess<TimeSpan, Error>(delayToNextRun => Task.Delay(delayToNextRun, stoppingToken).Wait(stoppingToken))
            //.TrySafe(_ => addNewFilesService.StartAsync(stoppingToken).Wait(stoppingToken));

            await Task.Delay(50_000, stoppingToken);
        }
    }
}
