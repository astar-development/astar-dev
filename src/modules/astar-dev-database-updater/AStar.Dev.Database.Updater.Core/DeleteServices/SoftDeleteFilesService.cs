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
// public class SoftDeleteFilesService(FilesContext context, DeleteFileService deleteFileService, DbContextSaveService dbContextSaveService, ILogger<SoftDeleteFilesService> logger)
// {
//     /// <summary>
//     ///     The StartAsync method is called by the runtime and will update the database with any new files
//     /// </summary>
//     /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
//     public async Task StartAsync(CancellationToken stoppingToken)
//     {
//         logger.LogInformation("Starting removal of files marked for soft deletion");
//
//         var fileAccessDetails = await context.FileAccessDetails.AsNoTracking().Where(fileAccess => fileAccess.SoftDeletePending)
//                                              .ToListAsync(stoppingToken);
//
//         logger.LogInformation("There are {Files} files marked for soft deletion", fileAccessDetails.Count);
//
//         foreach (var fileAccessDetail in fileAccessDetails)
//         {
//             var fileDetail = await context.Files.AsNoTracking().SingleAsync(file => file.FileAccessDetail.Id == fileAccessDetail.Id, stoppingToken);
//
//             logger.LogInformation("Soft-deleting file: {FileName} from {DirectoryName}", fileDetail.FileName, fileDetail.DirectoryName);
//
//             deleteFileService.DeleteFileIfItExists(fileDetail);
//
//             fileAccessDetail.SoftDeletePending = false;
//             fileAccessDetail.SoftDeleted       = true;
//         }
//
//         await dbContextSaveService.SaveChangesSafely(stoppingToken);
//         logger.LogInformation("Completed removal of files marked for soft deletion");
//     }
// }

