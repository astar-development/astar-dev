using System.IO.Abstractions;
using AStar.Dev.Functional.Extensions;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AStar.Dev.Database.Updater.Core.Files;

/// <summary>
/// </summary>
/// <param name="serviceScopeFactory"></param>
public class FileListService(IServiceScopeFactory serviceScopeFactory)
{
    /// <summary>
    /// </summary>
    /// <param name="path"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    public async Task<Result<IReadOnlyCollection<FileDetail>, ErrorResponse>> Get(string path, CancellationToken stoppingToken)
    {
        var       enumerationOptions = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = true, ReturnSpecialDirectories = false };
        using var scope              = serviceScopeFactory.CreateScope();
        var       fileSystem         = scope.ServiceProvider.GetRequiredService<IFileSystem>();

        var filesContext             = scope.ServiceProvider.GetRequiredService<FilesContext>();
        var filePaths                = fileSystem.Directory.GetFiles(path, "*", enumerationOptions);
        var filesAlreadyInTheContext = await filesContext.Files.AsNoTracking().Select(f => f.FullNameWithPath).ToListAsync(stoppingToken);
        var filesNotInTheDatabase    = filePaths.Except(filesAlreadyInTheContext).ToArray();

        var filesToProcessCount = filesNotInTheDatabase.Length;
        var filesToProcess      = new List<FileDetail>(filesToProcessCount);
        filesToProcess.AddRange(filesNotInTheDatabase.Select(file => fileSystem.FileInfo.New(file)).Select(fileInfo => new FileDetail(fileInfo) { Id = new() { Value = Guid.CreateVersion7() } }));

        return new Result<IReadOnlyCollection<FileDetail>, ErrorResponse>.Ok(filesToProcess);
    }
}
