namespace AStar.Dev.Infrastructure.FilesDb.Models;

/// <summary>
///     Contains the supported Deletion Types
/// </summary>
public enum DeletionScope
{
    /// <summary>
    ///     Delete the file only
    /// </summary>
    DeleteFile,

    /// <summary>
    ///     Delete the containing directory
    /// </summary>
    DeleteDirectory,

    /// <summary>
    ///     Delete everything for the model
    /// </summary>
    DeleteModel
}
