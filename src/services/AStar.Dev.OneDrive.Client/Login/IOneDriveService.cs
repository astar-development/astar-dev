using AStar.Dev.Functional.Extensions;
using AStar.Dev.OneDrive.Client.ViewModels;
using Microsoft.Graph.Models;

namespace AStar.Dev.OneDrive.Client.Login;

public interface IOneDriveService
{
    Task RunFullSyncAsync(MainWindowViewModel vm, CancellationToken token);
    Task DownloadFilesAsync(MainWindowViewModel vm, CancellationToken token);
    Task<Result<DriveItem?, Exception>> GetItemByPathAsync(string path);
    Task<Result<Stream, Exception>> DownloadFileAsync(string path);
    Task<Result<DriveItem, Exception>> UploadFileAsync(string path, Stream content);
    Task<Result<DriveItem, Exception>> CreateFolderAsync(string folderPath, string folderName);
}
