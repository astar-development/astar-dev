// using AStar.Dev.Database.Updater.Core.Models;
// using AStar.Dev.Database.Updater.Core.Services;
// using AStar.Dev.Functional.Extensions;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
//
// namespace AStar.Dev.Database.Updater.Core.Files;
//
// /// <summary>
// ///     The <see cref="AddNewFilesBackgroundService" /> class
// /// </summary>
// /// <param name="softDeleteFilesService">The <see cref="SoftDeleteFilesService" /> required to trigger the Soft Delete Files update</param>
// /// <param name="timeDelay">An instance of <see cref="TimeDelay" /> to control the start time of the process</param>
// /// <param name="config">An instance of the <see cref="DatabaseUpdaterConfiguration" /> options used to configure the addition of the new files</param>
// /// <param name="logger">An instance of the <see cref="ILogger" /> to log status / errors</param>
// public class SoftDeleteFilesBackgroundService(
//     SoftDeleteFilesService                    softDeleteFilesService,
//     TimeDelay                                 timeDelay,
//     IOptions<DatabaseUpdaterConfiguration>    config,
//     ILogger<SoftDeleteFilesBackgroundService> logger)
//     : BackgroundService
// {
//     /// <summary>
//     ///     The StartAsync method is called by the runtime and will delete any files marked for soft-deletion
//     /// </summary>
//     /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         await Task.CompletedTask;
//         var startAtTime        = config.Value.NewFilesScheduledTime;
//
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             timeDelay.CalculateDelayToNextRun(startAtTime)
//                      .OnSuccess<TimeSpan, Error>(delayToNextRun => logger.LogInformation("Waiting for: {DelayToNextRun} hours before updating the full database again.", delayToNextRun))
//                      .OnSuccess<TimeSpan, Error>(delayToNextRun => Task.Delay(delayToNextRun, stoppingToken).Wait(stoppingToken))
//                      .TrySafe(_ => softDeleteFilesService.StartAsync(stoppingToken).Wait(stoppingToken));
//         }
//     }
// }

