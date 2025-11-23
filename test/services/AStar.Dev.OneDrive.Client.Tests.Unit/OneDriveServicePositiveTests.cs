using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
    public class OneDriveServicePositiveTests
    {
        [Fact]
        public async Task GetRootItemsAsync_Returns_Items_When_DriveHasChildren()
        {
            // Arrange: create a Drive with root children
            var child1 = new DriveItem { Id = "1", Name = "file1.txt" };
            var child2 = new DriveItem { Id = "2", Name = "file2.txt" };

            var drive = new Drive
            {
                Id = "drive-id",
                Root = new DriveItem
                {
                    Id = "root-id",
                    Children = new List<DriveItem> { child1, child2 }
                }
            };

            // responder returns the Drive when a Drive is requested
            Task<object?> Responder(RequestInformation req, Type responseType, CancellationToken ct)
            {
                if (responseType == typeof(Drive) || responseType == typeof(Microsoft.Graph.Drive))
                {
                    return Task.FromResult<object?>(drive);
                }
                return Task.FromResult<object?>(null);
            }

            var graphClient = TestUtilities.CreateGraphClient(Responder);
            var login = new Fakes.FakeLoginService(graphClient);
            var svc = new OneDriveService(login, NullLogger<OneDriveService>.Instance);

            // Act
            var result = await svc.GetRootItemsAsync();

            // Assert
            result.Should().BeOfType<Result<List<DriveItem>, Exception>.Ok>();
            var ok = (Result<List<DriveItem>, Exception>.Ok)result;
            ok.Value.Should().HaveCount(2);
            ok.Value[0].Name.Should().Be("file1.txt");
            ok.Value[1].Name.Should().Be("file2.txt");
        }

        [Fact]
        public async Task DownloadFileAsync_Returns_Stream_When_FileExists()
        {
            // Arrange
            var expectedContent = "hello world";
            var drive = new Drive { Id = "drive-id" };

            Task<object?> Responder(RequestInformation req, Type responseType, CancellationToken ct)
            {
                if (responseType == typeof(Drive) )
                    return Task.FromResult<object?>(drive);
                if (responseType == typeof(System.IO.Stream))
                    return Task.FromResult<object?>(TestUtilities.StreamFromString(expectedContent));
                return Task.FromResult<object?>(null);
            }

            var graphClient = TestUtilities.CreateGraphClient(Responder);
            var login = new Fakes.FakeLoginService(graphClient);
            var svc = new OneDriveService(login, NullLogger<OneDriveService>.Instance);

            // Act
            var result = await svc.DownloadFileAsync("/some/path/file.txt");

            // Assert
            result.Should().BeOfType<Result<System.IO.Stream, Exception>.Ok>();
            var ok = (Result<System.IO.Stream, Exception>.Ok)result;
            using var sr = new System.IO.StreamReader(ok.Value);
            var text = await sr.ReadToEndAsync();
            text.Should().Be(expectedContent);
        }

        [Fact]
        public async Task UploadFileAsync_Returns_DriveItem_OnSuccess()
        {
            // Arrange
            var drive = new Drive { Id = "drive-id" };
            var uploaded = new DriveItem { Id = "new-id", Name = "uploaded.txt" };

            Task<object?> Responder(RequestInformation req, Type responseType, CancellationToken ct)
            {
                if (responseType == typeof(Drive))
                    return Task.FromResult<object?>(drive);
                if (responseType == typeof(DriveItem))
                    return Task.FromResult<object?>(uploaded);
                return Task.FromResult<object?>(null);
            }

            var graphClient = TestUtilities.CreateGraphClient(Responder);
            var login = new Fakes.FakeLoginService(graphClient);
            var svc = new OneDriveService(login, NullLogger<OneDriveService>.Instance);

            // Act
            using var ms = TestUtilities.StreamFromString("content");
            var result = await svc.UploadFileAsync("/some/path/uploaded.txt", ms);

            // Assert
            result.Should().BeOfType<Result<DriveItem, Exception>.Ok>();
            var ok = (Result<DriveItem, Exception>.Ok)result;
            ok.Value.Id.Should().Be("new-id");
            ok.Value.Name.Should().Be("uploaded.txt");
        }

        [Fact]
        public async Task CreateFolderAsync_Returns_CreatedDriveItem()
        {
            // Arrange
            var drive = new Drive { Id = "drive-id" };
            var created = new DriveItem { Id = "folder-id", Name = "NewFolder" };

            Task<object?> Responder(RequestInformation req, Type responseType, CancellationToken ct)
            {
                if (responseType == typeof(Drive))
                    return Task.FromResult<object?>(drive);
                if (responseType == typeof(DriveItem))
                    return Task.FromResult<object?>(created);
                return Task.FromResult<object?>(null);
            }

            var graphClient = TestUtilities.CreateGraphClient(Responder);
            var login = new Fakes.FakeLoginService(graphClient);
            var svc = new OneDriveService(login, NullLogger<OneDriveService>.Instance);

            // Act
            var result = await svc.CreateFolderAsync("", "NewFolder");

            // Assert
            result.Should().BeOfType<Result<DriveItem, Exception>.Ok>();
            var ok = (Result<DriveItem, Exception>.Ok)result;
            ok.Value.Id.Should().Be("folder-id");
            ok.Value.Name.Should().Be("NewFolder");
        }
    }
}
