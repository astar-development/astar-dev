// using AStar.Dev.Infrastructure.FilesDb.Data;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
//
// namespace AStar.Dev.Database.Updater.Core.Files;
//
// /// <summary>
// /// </summary>
// /// <param name="context"></param>
// /// <param name="deleteFileService"></param>
// /// <param name="dbContextSaveService"></param>
// /// <param name="logger"></param>
// public class HardDeleteFilesService(FilesContext context, DeleteFileService deleteFileService, DbContextSaveService dbContextSaveService, ILogger<HardDeleteFilesService> logger)
// {
//     /// <summary>
//     ///     The StartAsync method is called by the runtime and will update the database with any new files
//     /// </summary>
//     /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
//     public async Task StartAsync(CancellationToken stoppingToken)
//     {
//         logger.LogInformation("Starting removal of files marked for hard deletion");
//
//         var fileAccessDetails = await context.FileAccessDetails.Where(fileAccess => fileAccess.HardDeletePending)
//                                              .ToListAsync(stoppingToken);
//
//         logger.LogInformation("There are {Files} files marked for hard deletion", fileAccessDetails.Count);
//
//         foreach(var fileAccessDetail in fileAccessDetails)
//         {
//             var fileDetail = await context.Files.AsNoTracking().SingleAsync(file => file.FileAccessDetail.Id == fileAccessDetail.Id, stoppingToken);
//
//             logger.LogInformation("Hard-deleting file: {FileName} from {DirectoryName}", fileDetail.FileName, fileDetail.DirectoryName);
//
//             deleteFileService.DeleteFileIfItExists(fileDetail);
//
//             _ = fileAccessDetail.HardDeletePending = false;
//             _ = fileAccessDetail.HardDeleted       = true;
//         }
//
//         await dbContextSaveService.SaveChangesSafely(stoppingToken);
//         logger.LogInformation("Completed removal of files marked for hard deletion");
//     }
// }

