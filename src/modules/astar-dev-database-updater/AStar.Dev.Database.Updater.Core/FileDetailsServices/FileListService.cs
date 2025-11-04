using System.IO.Abstractions;
using AStar.Dev.Functional.Extensions;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AStar.Dev.Database.Updater.Core.FileDetailsServices;

/// <summary>
///     Service responsible for retrieving a list of file paths from a specified directory,
///     excluding those already present in the database context.
/// </summary>
public class FileListService(IFileSystem fileSystem, IServiceScopeFactory scopeFactory) : IFileListService
{
    /// <summary>
    ///     Retrieves a list of file details for the files located in the specified directory path that are not already present in the database.
    /// </summary>
    /// <param name="path">The root directory path to search for files.</param>
    /// <param name="stoppingToken">A token to observe for any cancellation requests.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a <see cref="Result{TSuccess, TError}" /> object with a collection of file details on success or an error response
    ///     on failure.
    /// </returns>
    public async Task<Result<List<FileDetail>, ErrorResponse>> Get(string path, CancellationToken stoppingToken)
    {
        var enumerationOptions = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = true, ReturnSpecialDirectories = false };
        FilesContext filesContext = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<FilesContext>();
        List<string> filesAlreadyInTheContext = await filesContext.Files.AsNoTracking().Select(f => f.FullNameWithPath).ToListAsync(stoppingToken);

        return GetFileList(path, enumerationOptions)
               .Map(files => files.Except(filesAlreadyInTheContext).ToArray())
               .Map(files =>
                    {
                        var filesToProcess = new List<FileDetail>(files.Length);

                        filesToProcess.AddRange(files.Select(file => fileSystem.FileInfo.New(file))
                            .Select(fileInfo => new FileDetail(fileInfo) { Id = new FileId { Value = Guid.CreateVersion7() } }));

                        return filesToProcess;
                    });
    }

    private Result<string[], ErrorResponse> GetFileList(string path, EnumerationOptions enumerationOptions)
        => Try.Run(() => fileSystem.Directory.EnumerateFiles(path, "*", enumerationOptions).ToArray())
              .ToErrorResponse();
}
