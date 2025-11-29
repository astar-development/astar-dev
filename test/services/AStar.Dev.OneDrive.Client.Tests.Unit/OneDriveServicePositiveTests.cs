using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions;
using Microsoft.Graph.Models;
using Xunit;
using AStar.Dev.OneDrive.Client.Services;
using AStar.Dev.OneDrive.Client.Tests.Unit.Fakes;
using AStar.Dev.Functional.Extensions;

namespace AStar.Dev.OneDrive.Client.Tests.Unit
{
    public sealed class OneDriveServicePositiveTests
    {
        // [Fact]
        // public async Task GetRootItemsAsync_Returns_Items_When_DriveHasChildren()
        // {
        //     // Arrange: create a Drive with root children
        //     var child1 = new DriveItem { Id = "1", Name = "file1.txt" };
        //     var child2 = new DriveItem { Id = "2", Name = "file2.txt" };
        //
        //     var drive = new Drive
        //     {
        //         Id = "drive-id",
        //         Root = new DriveItem
        //         {
        //             Id = "root-id",
        //             Children = new List<DriveItem> { child1, child2 }
        //         }
        //     };
        //
        //     // responder returns the Drive when a Drive is requested
        //     Task<object?> Responder(RequestInformation req, Type responseType, CancellationToken ct)
        //     {
        //         if (responseType == typeof(Microsoft.Graph.Models.Drive)) return Task.FromResult<object?>(drive);
        //         return Task.FromResult<object?>(null);
        //     }
        //
        //     GraphServiceClient graphClient = TestUtilities.CreateGraphClient(Responder);
        //     var login = new Fakes.FakeLoginService(graphClient);
        //     var svc = new OneDriveService(login, new(), NullLogger<OneDriveService>.Instance);
        //
        //     // Act
        //     var result = await svc.GetRootItemsAsync();
        //
        //     // Assert
        //     result.ShouldBeOfType<Result<List<DriveItem>, Exception>.Ok>();
        //     var ok = (Result<List<DriveItem>, Exception>.Ok)result;
        //     ok.Value.ShouldBe(2);
        //     ok.Value[0].Name.ShouldBe("file1.txt");
        //     ok.Value[1].Name.ShouldBe("file2.txt");
        // }
        //
        // [Fact]
        // public async Task DownloadFileAsync_Returns_Stream_When_FileExists()
        // {
        //     // Arrange
        //     var expectedContent = "hello world";
        //     var drive = new Drive { Id = "drive-id" };
        //
        //     Task<object?> Responder(RequestInformation req, Type responseType, CancellationToken ct)
        //     {
        //         if (responseType == typeof(Microsoft.Graph.Models.Drive) )
        //             return Task.FromResult<object?>(drive);
        //         if (responseType == typeof(System.IO.Stream))
        //             return Task.FromResult<object?>(TestUtilities.StreamFromString(expectedContent));
        //         return Task.FromResult<object?>(null);
        //     }
        //
        //     GraphServiceClient graphClient = TestUtilities.CreateGraphClient(Responder);
        //     var login = new Fakes.FakeLoginService(graphClient);
        //     var svc = new OneDriveService(login, new(), NullLogger<OneDriveService>.Instance);
        //
        //     // Act
        //     Result<Stream, Exception> result = await svc.DownloadFileAsync("/some/path/file.txt");
        //
        //     // Assert
        //     result.ShouldBeOfType<Result<System.IO.Stream, Exception>.Ok>();
        //     var ok = (Result<System.IO.Stream, Exception>.Ok)result;
        //     using var sr = new System.IO.StreamReader(ok.Value);
        //     var text = await sr.ReadToEndAsync();
        //     text.ShouldBe(expectedContent);
        // }

        [Fact]
        public async Task UploadFileAsync_Returns_DriveItem_OnSuccess()
        {
            // Arrange
            var drive = new Drive { Id = "drive-id" };
            var uploaded = new DriveItem { Id = "new-id", Name = "uploaded.txt" };

            Task<object?> Responder(RequestInformation req, Type responseType, CancellationToken ct)
            {
                if (responseType == typeof(Microsoft.Graph.Models.Drive))
                    return Task.FromResult<object?>(drive);
                if (responseType == typeof(Microsoft.Graph.Models.DriveItem))
                    return Task.FromResult<object?>(uploaded);
                return Task.FromResult<object?>(null);
            }

            GraphServiceClient graphClient = TestUtilities.CreateGraphClient(Responder);
            var login = new Fakes.FakeLoginService(graphClient);
            var svc = new OneDriveService(login, new(), NullLogger<OneDriveService>.Instance);

            // Act
            using MemoryStream ms = TestUtilities.StreamFromString("content");
            Result<DriveItem, Exception> result = await svc.UploadFileAsync("/some/path/uploaded.txt", ms);

            // Assert
            result.ShouldBeOfType<Result<DriveItem, Exception>.Ok>();
            var ok = (Result<DriveItem, Exception>.Ok)result;
            ok.Value.Id.ShouldBe("new-id");
            ok.Value.Name.ShouldBe("uploaded.txt");
        }

        [Fact]
        public async Task CreateFolderAsync_Returns_CreatedDriveItem()
        {
            // Arrange
            var drive = new Drive { Id = "drive-id" };
            var created = new DriveItem { Id = "folder-id", Name = "NewFolder" };

            Task<object?> Responder(RequestInformation req, Type responseType, CancellationToken ct)
            {
                if (responseType == typeof(Microsoft.Graph.Models.Drive))
                    return Task.FromResult<object?>(drive);
                if (responseType == typeof(Microsoft.Graph.Models.DriveItem))
                    return Task.FromResult<object?>(created);
                return Task.FromResult<object?>(null);
            }

            GraphServiceClient graphClient = TestUtilities.CreateGraphClient(Responder);
            var login = new Fakes.FakeLoginService(graphClient);
            var svc = new OneDriveService(login, new(), NullLogger<OneDriveService>.Instance);

            // Act
            Result<DriveItem, Exception> result = await svc.CreateFolderAsync("", "NewFolder");

            // Assert
            result.ShouldBeOfType<Result<DriveItem, Exception>.Ok>();
            var ok = (Result<DriveItem, Exception>.Ok)result;
            ok.Value.Id.ShouldBe("folder-id");
            ok.Value.Name.ShouldBe("NewFolder");
        }
    }
}
