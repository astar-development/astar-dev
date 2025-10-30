using System.Globalization;
using System.Text.RegularExpressions;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Utilities;

namespace AStar.Dev.FileServices.Common;

/// <summary>
/// </summary>
public class FileHandleService
{
    /// <summary>
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <param name="fileHandlesAlreadyInTheContext"></param>
    /// <returns></returns>
    public FileHandle GenerateFileHandle(FileDetail fileInfo, List<FileHandle> fileHandlesAlreadyInTheContext)
    {
        var fileHandle = GenerateFileHandle(fileInfo.FileName.Value).Value.TruncateIfRequired(350);

        if(fileHandlesAlreadyInTheContext.Any(h => h.Value == fileHandle)) fileHandle = $"{Guid.CreateVersion7()}-{fileHandle}".TruncateIfRequired(350);

        var newHandle = FileHandle.Create(fileHandle);
        fileHandlesAlreadyInTheContext.Add(newHandle);

        return newHandle;
    }

    private FileHandle GenerateFileHandle(string file)
    {
        var newHandle = Regex.Replace(Path.GetInvalidPathChars()
                                          .Aggregate(file, (current, illegalCharacter) => current.Replace(illegalCharacter, '-'))
                                          .Replace("/", "-")
                                          .Replace(" ", "-")
                                          .Replace("_", "-")
                                          .Replace("(", "-")
                                          .Replace(")", "-")
                                          .Replace("+", "-")
                                          .Replace(".", "-")
                                          .Replace(",", "-")
                                          .ToLower(CultureInfo.InvariantCulture), @"(-+)", "-", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

        return FileHandle.Create(newHandle.StartsWith("-", StringComparison.OrdinalIgnoreCase)
                                     ? newHandle[1..]
                                     : newHandle);
    }
}
