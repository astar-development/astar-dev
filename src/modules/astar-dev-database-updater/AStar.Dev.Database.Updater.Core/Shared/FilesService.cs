// using System.IO.Abstractions;
// using System.Text.Json;
// using AStar.Dev.Database.Updater.Core.Models;
// using AStar.Dev.Infrastructure.FilesDb.Data;
// using AStar.Dev.Infrastructure.FilesDb.Models;
// using AStar.Dev.Technical.Debt.Reporting;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using SkiaSharp;
//
// namespace AStar.Dev.Database.Updater.Core.Shared;
//
// /// <summary>
// /// </summary>
// /// <param name="context"></param>
// /// <param name="fileSystem"></param>
// /// <param name="logger"></param>
// [Refactor(1, 1, "Not sure if used")]
// public sealed class FilesService(FilesContext context, IFileSystem fileSystem, ILogger<FilesService> logger)
// {
//     private readonly List<FileClassification>    cachedFileClassifications = [];
//     private          List<ClassificationMapping> classificationMappings    = [];
//
//     /// <summary>
//     /// </summary>
//     /// <param name="dir"></param>
//     /// <param name="searchPattern"></param>
//     /// <param name="recursive"></param>
//     /// <returns></returns>
//     public IEnumerable<string> GetFilesFromDirectory(string dir, string searchPattern = "*.*", bool recursive = true)
//     {
//         logger.LogInformation("Getting files in {Directory}", dir);
//
//         var files = fileSystem.Directory.GetFiles(dir, searchPattern,
//                                                   new EnumerationOptions { RecurseSubdirectories = recursive, IgnoreInaccessible = true }
//                                                  );
//
//         logger.LogInformation("Got files in {Directory}", dir);
//
//         return files;
//     }
//
//     /// <summary>
//     /// </summary>
//     /// <param name="directories"></param>
//     /// <returns></returns>
//     public List<string> GetFiles(string[] directories)
//     {
//         var files = new List<string>();
//
//         foreach (var dir in directories)
//         {
//             files.AddRange(GetFilesFromDirectory(dir));
//         }
//
//         return files;
//     }
//
//     /// <summary>
//     /// </summary>
//     /// <param name="files"></param>
//     /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
//     public async Task ProcessNewFiles(IEnumerable<string> files, CancellationToken stoppingToken)
//     {
//         classificationMappings = await AddClassificationMappingsAsync(stoppingToken);
//
//         foreach (var classificationMapping in classificationMappings)
//         {
//             if (await context.FileClassifications.FirstOrDefaultAsync(c => c.Name == classificationMapping.DatabaseMapping, stoppingToken) is not null)
//             {
//                 continue;
//             }
//
//             await context.FileClassifications.AddAsync(new () { Name = classificationMapping.DatabaseMapping }, stoppingToken);
//             await context.SaveChangesAsync(stoppingToken);
//         }
//
//         var fileClassifications = await context.FileClassifications.ToListAsync(stoppingToken);
//
//         foreach (var fileClassification in fileClassifications)
//         {
//             cachedFileClassifications.Add(fileClassification);
//         }
//
//         var filesInDb        = context.Files.AsNoTracking().Select(file => Path.Combine(file.DirectoryName, file.FileName));
//         var notInTheDatabase = files.Except(filesInDb).ToList();
//
//         await ProcessFilesNotInTheDatabase(notInTheDatabase, stoppingToken);
//
//         await SaveChangesSafely(stoppingToken);
//     }
//
//
//     private async Task<List<ClassificationMapping>> AddClassificationMappingsAsync(CancellationToken stoppingToken)
//     {
//         var fileContent = await fileSystem.File.ReadAllTextAsync(@"c:\temp\image-mappings.json", stoppingToken);
//
//         var mappings = JsonSerializer.Deserialize<List<ClassificationMapping>>(fileContent);
//
//         return mappings ?? [];
//     }
//
//     private async Task ProcessFilesNotInTheDatabase(List<string> notInTheDatabase, CancellationToken stoppingToken)
//     {
//         var counter = 0;
//
//         foreach (var file in notInTheDatabase)
//         {
//             if (stoppingToken.IsCancellationRequested)
//             {
//                 await SaveChangesSafely(stoppingToken);
//
//                 break;
//             }
//
//             var lastIndexOf   = file.LastIndexOf('\\');
//             var directoryName = file[..lastIndexOf];
//             var fileName      = file[++lastIndexOf..];
//
//             if (await context.Files.AsNoTracking().AnyAsync(fileDetail => fileDetail.FileName == fileName && fileDetail.DirectoryName == directoryName,
//                                                             stoppingToken))
//             {
//                 continue;
//             }
//
//             await AddNewFileAsync(file, context, stoppingToken);
//             counter++;
//
//             if (counter < 100)
//             {
//                 continue;
//             }
//
//             counter = 0;
//             await SaveChangesSafely(stoppingToken);
//             var count = context.Files.Count();
//
//             logger.LogInformation("Updated the database. File {FileName} has been added to the database. There are now {FileCount} files in the database", file, count);
//         }
//     }
//
//     /// <summary>
//     /// </summary>
//     /// <param name="files"></param>
//     /// <param name="directories"></param>
//     /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
//     public async Task ProcessMovedFiles(IEnumerable<string> files, string[] directories, CancellationToken stoppingToken)
//     {
//         logger.LogInformation("Starting update of files that have moved");
//         var filesAsArray = files.ToArray();
//
//         foreach (var directory in directories)
//         foreach (var file in filesAsArray.Where(file => file.StartsWith(directory)))
//         {
//             var lastIndexOf   = file.LastIndexOf('\\');
//             var directoryName = file[..lastIndexOf];
//             var fileName      = file[++lastIndexOf..];
//
//             var movedFile = await context.Files.FirstOrDefaultAsync(
//                                                                     f => f.DirectoryName.Value.StartsWith(directory) && f.DirectoryName != directoryName &&
//                                                                          f.FileName                                               == fileName, stoppingToken);
//
//             if (movedFile != null)
//             {
//                 await UpdateExistingFile(directoryName, fileName, movedFile, stoppingToken);
//             }
//         }
//
//         await SaveChangesSafely(stoppingToken);
//     }
//
//     /// <summary>
//     /// </summary>
//     /// <param name="files"></param>
//     /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
//     public async Task RemoveFilesFromDbThatDoNotExistAnyMore(IEnumerable<string> files, CancellationToken stoppingToken)
//     {
//         logger.LogInformation("Starting removal of files deleted from disc outside of the UI.");
//
//         var filesInDb = context.Files
//                                .Include(x => x.FileAccessDetail)
//                                .Where(file => !file.FileAccessDetail.SoftDeleted && !file.FileAccessDetail.SoftDeletePending &&
//                                               !file.FileAccessDetail.HardDeletePending)
//                                .Select(file => Path.Combine(file.DirectoryName, file.FileName));
//
//         var notOnDisc = await filesInDb.Except(files).ToListAsync(stoppingToken);
//
//         foreach (var file in notOnDisc)
//         {
//             var lastIndexOf   = file.LastIndexOf('\\');
//             var directoryName = file[..lastIndexOf];
//             var fileName      = file[++lastIndexOf..];
//
//             var fileDetail = await context.Files.SingleAsync(f => f.DirectoryName == directoryName && f.FileName == fileName,
//                                                              stoppingToken);
//
//             var fileCount             = await context.Files.CountAsync(stoppingToken);
//             var fileAccessDetailCount = await context.FileAccessDetails.CountAsync(stoppingToken);
//             _ = context.Files.Remove(fileDetail);
//             await SaveChangesSafely(stoppingToken);
//             var fileCountAfter             = await context.Files.CountAsync(stoppingToken);
//             var fileAccessDetailCountAfter = await context.FileAccessDetails.CountAsync(stoppingToken);
//
//             logger.LogInformation(
//                                   "File Count before: {FileCount} File Access Detail Count before: {FileAccessDetailCount}, File Count after: {FileCountAfter} File Access Detail Count after: {FileAccessDetailCountAfter}",
//                                   fileCount, fileAccessDetailCount, fileCountAfter, fileAccessDetailCountAfter);
//         }
//
//         await SaveChangesSafely(stoppingToken);
//     }
//
//     private async Task UpdateExistingFile(string            directoryName,
//                                           string            fileName,
//                                           FileDetail        fileFromDatabase,
//                                           CancellationToken stoppingToken)
//     {
//         foreach (var file in context.Files.Where(file => file.FileName == fileName))
//         {
//             _ = context.Files.Remove(file);
//         }
//
//         await SaveChangesSafely(stoppingToken);
//
//         var updatedFile = new FileDetail
//                           {
//                               DirectoryName    = directoryName,
//                               Height           = fileFromDatabase.Height,
//                               Width            = fileFromDatabase.Width,
//                               FileName         = fileName,
//                               FileSize         = fileFromDatabase.FileSize,
//                               FileAccessDetail = new() { SoftDeleted = false, SoftDeletePending = false, DetailsLastUpdated = DateTime.UtcNow  }
//                           };
//
//         _ = await context.Files.AddAsync(updatedFile, stoppingToken);
//
//         logger.LogInformation(
//                               "File: {FileName} ({OriginalLocation}) appears to have moved since being added to the dB - previous location: {DirectoryName}",
//                               fileName, directoryName, fileFromDatabase.DirectoryName);
//     }
//
//
//     private async Task AddNewFileAsync( string file, FilesContext filesContext, CancellationToken stoppingToken)
//     {
//         var fileClassifications = await GetFileClassificationsFromFilenameAsync(file, filesContext, stoppingToken);
//
//         try
//         {
//             var fileInfo   = fileSystem.FileInfo.New(file);
//             var fileDetail = new FileDetail { FileName = fileInfo.Name, DirectoryName = fileInfo.DirectoryName!, FileSize = fileInfo.Length };
//
//             if (fileDetail.IsImage)
//             {
//                 var image = SKImage.FromEncodedData(file);
//
//                 if (image is null)
//                 {
//                     File.Delete(file);
//                 }
//                 else
//                 {
//                     fileDetail.Height = image.Height;
//                     fileDetail.Width  = image.Width;
//                 }
//             }
//
//             var fileAccessDetail = new FileAccessDetail { SoftDeleted = false, SoftDeletePending = false, DetailsLastUpdated = DateTime.UtcNow };
//
//             fileDetail.FileAccessDetail    = fileAccessDetail;
//             fileDetail.FileClassifications = fileClassifications;
//             _                              = context.Files.Add(fileDetail);
//             await SaveChangesSafely(stoppingToken);
//         }
//         catch (Exception ex)
//         {
//             logger.LogError(ex, "Error retrieving file '{File}' details", file);
//         }
//     }
//
//     private async Task<List<FileClassification>> GetFileClassificationsFromFilenameAsync(string fileName, FilesContext filesContext, CancellationToken stoppingToken)
//     {
//         List<FileClassification> fileClassifications = [];
//
//         foreach (var classificationMapping in classificationMappings.Where(classificationMapping => FileName.Value.Contains(classificationMapping.FileNameContains)))
//         {
//             var  classification = await GetOrCreateFileClassificationAsync(filesContext, classificationMapping.DatabaseMapping, stoppingToken);
//
//             if (fileClassifications.Any(fileClasssification => fileClasssification.Id == classification.Id))
//             {
//                 continue;
//             }
//
//             fileClassifications.Add(classification);
//         }
//
//         return fileClassifications;
//     }
//
//     [Refactor(2, 4, "Look at replacing with the Find method as we have the same context")]
//     private async Task<FileClassification> GetOrCreateFileClassificationAsync(FilesContext filesContext, string classificationName, CancellationToken stoppingToken)
//     {
//         var local = cachedFileClassifications.SingleOrDefault(f => f.Name == classificationName);
//
//         if (local is not null)
//         {
//             return local;
//         }
//
//         var fileClassification = await filesContext.FileClassifications.SingleOrDefaultAsync(x => x.Name.Contains(classificationName), stoppingToken);
//
//         if (fileClassification is not null)
//         {
//             cachedFileClassifications.Add(fileClassification);
//
//             return fileClassification;
//         }
//
//         await filesContext.FileClassifications.AddAsync(new () { Name = classificationName }, stoppingToken);
//         await filesContext.SaveChangesAsync(stoppingToken);
//         local = (await filesContext.FileClassifications.SingleOrDefaultAsync(x => x.Name.Contains(classificationName), stoppingToken))!;
//
//         cachedFileClassifications.Add(local);
//
//         return local;
//     }
//
//     private async Task SaveChangesSafely(CancellationToken stoppingToken)
//     {
//         try
//         {
//             await context.SaveChangesAsync(stoppingToken);
//         }
//         catch (DbUpdateException ex)
//         {
//             if (!ex.Message.StartsWith("The database operation was expected to affect"))
//             {
//                 logger.LogError(ex, "Error: {Error} occurred whilst saving changes - probably 'no records affected'",
//                                 ex.Message);
//             }
//         }
//     }
// }

