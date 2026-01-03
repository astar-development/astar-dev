// using System;
// using System.Threading.Tasks;
//
// using Xunit;
// using AStar.Dev.Functional.Extensions;
// using AStar.Dev.OneDrive.Client.Services;
// using Microsoft.Graph;
//
// namespace AStar.Dev.OneDrive.Client.Tests.Unit
// {
//     public sealed class OneDriveServiceNegativeTests
//     {
//         private sealed class FailingLoginService : ILoginService
//         {
//             private readonly Exception _ex;
//             public FailingLoginService(Exception ex) => _ex = ex;
//             public bool IsSignedIn => false;
//             public Task<Result<GraphServiceClient, Exception>> SignInAsync() => Task.FromResult<Result<GraphServiceClient, Exception>>(new Result<GraphServiceClient, Exception>.Error(_ex));
//             public Task<Result<bool, Exception>> SignOutAsync() => Task.FromResult<Result<bool, Exception>>(new Result<bool, Exception>.Ok(true));
//         }
//
//         [Fact]
//         public async Task GetItemByPathAsync_Returns_Error_When_Login_Fails()
//         {
//             var ex = new InvalidOperationException("login failed");
//             var svc = new OneDriveService(new FailingLoginService(ex), Microsoft.Extensions.Logging.Abstractions.NullLogger<OneDriveService>.Instance);
//
//             var result = await svc.GetItemByPathAsync("/some/path");
//             result.ShouldBeOfType<Result<Microsoft.Graph.Models.DriveItem?, Exception>.Error>();
//         }
//
//         [Fact]
//         public async Task DownloadFileAsync_Returns_Error_When_Login_Fails()
//         {
//             var ex = new InvalidOperationException("login failed");
//             var svc = new OneDriveService(new FailingLoginService(ex), Microsoft.Extensions.Logging.Abstractions.NullLogger<OneDriveService>.Instance);
//
//             var result = await svc.DownloadFileAsync("/some/file.txt");
//             result.ShouldBeOfType<Result<System.IO.Stream, Exception>.Error>();
//         }
//
//         [Fact]
//         public async Task UploadFileAsync_Returns_Error_When_Login_Fails()
//         {
//             var ex = new InvalidOperationException("login failed");
//             var svc = new OneDriveService(new FailingLoginService(ex), Microsoft.Extensions.Logging.Abstractions.NullLogger<OneDriveService>.Instance);
//
//             using var ms = new System.IO.MemoryStream();
//             var result = await svc.UploadFileAsync("/upload.txt", ms);
//             result.ShouldBeOfType<Result<Microsoft.Graph.Models.DriveItem, Exception>.Error>();
//         }
//
//         [Fact]
//         public async Task CreateFolderAsync_Returns_Error_When_Login_Fails()
//         {
//             var ex = new InvalidOperationException("login failed");
//             var svc = new OneDriveService(new FailingLoginService(ex), Microsoft.Extensions.Logging.Abstractions.NullLogger<OneDriveService>.Instance);
//
//             var result = await svc.CreateFolderAsync("/", "newfolder");
//             result.ShouldBeOfType<Result<Microsoft.Graph.Models.DriveItem, Exception>.Error>();
//         }
//     }
// }
