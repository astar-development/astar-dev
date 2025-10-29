// using System.IO.Abstractions;
// using AStar.Dev.Database.Updater.Core.Services;
// using AStar.Dev.Files.Api.Client.SDK.Models;
// using Microsoft.Extensions.Logging;
//
// namespace AStar.Dev.Database.Updater.Core.Files;
//
// /// <summary>
// /// </summary>
// /// <param name="fileSystem"></param>
// /// <param name="logger"></param>
// public class DeleteFileService(IFileSystem fileSystem, ILogger<FilesService> logger)
// {
//     /// <summary>
//     /// </summary>
//     /// <param name="fileDetail"></param>
//     public void DeleteFileIfItExists(FileDetail fileDetail)
//     {
//         try
//         {
//             if (fileSystem.File.Exists(fileDetail.FullNameWithPath))
//             {
//                 fileSystem.File.Delete(fileDetail.FullNameWithPath);
//             }
//         }
//         catch (DirectoryNotFoundException ex)
//         {
//             logger.LogWarning(ex, "Directory not found: {FullNameWithPath}", fileDetail.FullNameWithPath);
//         }
//         catch (FileNotFoundException ex)
//         {
//             logger.LogWarning(ex, "File not found: {FullNameWithPath}", fileDetail.FullNameWithPath);
//         }
//     }
// }

