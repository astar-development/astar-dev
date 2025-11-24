using System;
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
// using AStar.Dev.OneDrive.Client.Tests.Unit.Utilities;

namespace AStar.Dev.OneDrive.Client.Tests.Unit
{
    public sealed class OneDriveServiceErrorTests
    {
        [Fact]
        public async Task DownloadFileAsync_Returns_Error_When_AdapterThrows()
        {
            // Arrange
            var drive = new Drive { Id = "drive-id" };

            Task<object?> Responder(RequestInformation req, Type responseType, CancellationToken ct)
            {
                    if (responseType == typeof(Microsoft.Graph.Models.Drive))
                    return Task.FromResult<object?>(drive);
                // Simulate an API failure
                throw new InvalidOperationException("simulated api failure");
            }

            var client = TestUtilities.CreateGraphClient(Responder);
            var login = new FakeLoginService(client);
            var svc = new OneDriveService(login, NullLogger<OneDriveService>.Instance);

            // Act
            var result = await svc.DownloadFileAsync("/path/file.txt");

            // Assert
            result.Should().BeOfType<Result<System.IO.Stream, Exception>.Error>();
            ((Result<System.IO.Stream, Exception>.Error)result).Reason.Message.Should().Contain("simulated api failure");
        }

        [Fact]
        public async Task UploadFileAsync_Returns_Error_When_AdapterThrows()
        {
            // Arrange
            var drive = new Drive { Id = "drive-id" };

            Task<object?> Responder(RequestInformation req, Type responseType, CancellationToken ct)
            {
                    if (responseType == typeof(Microsoft.Graph.Models.Drive))
                    return Task.FromResult<object?>(drive);
                throw new Exception("upload error");
            }

            var client = TestUtilities.CreateGraphClient(Responder);
            var login = new FakeLoginService(client);
            var svc = new OneDriveService(login, NullLogger<OneDriveService>.Instance);

            using var ms = TestUtilities.StreamFromString("data");
            var result = await svc.UploadFileAsync("/path/upload.txt", ms);

            result.Should().BeOfType<Result<DriveItem, Exception>.Error>();
            ((Result<DriveItem, Exception>.Error)result).Reason.Message.Should().Contain("upload error");
        }

        [Fact]
        public async Task CreateFolderAsync_Returns_Error_When_AdapterThrows()
        {
            // Arrange
            var drive = new Drive { Id = "drive-id" };

            Task<object?> Responder(RequestInformation req, Type responseType, CancellationToken ct)
            {
                    if (responseType == typeof(Microsoft.Graph.Models.Drive))
                    return Task.FromResult<object?>(drive);
                throw new Exception("create folder failed");
            }

            var client = TestUtilities.CreateGraphClient(Responder);
            var login = new FakeLoginService(client);
            var svc = new OneDriveService(login, NullLogger<OneDriveService>.Instance);

            var result = await svc.CreateFolderAsync("", "NewFolder");

            result.Should().BeOfType<Result<DriveItem, Exception>.Error>();
            ((Result<DriveItem, Exception>.Error)result).Reason.Message.Should().Contain("create folder failed");
        }

        [Fact]
        public async Task Methods_Return_Error_When_LoginFails()
        {
            // Arrange
            var loginFail = new FailingLoginService(new Exception("login refused"));
            var svc = new OneDriveService(loginFail, NullLogger<OneDriveService>.Instance);

            // Act
            var r1 = await svc.GetRootItemsAsync();
            var r2 = await svc.DownloadFileAsync("/a");
            var r3 = await svc.UploadFileAsync("/a", TestUtilities.StreamFromString(""));
            var r4 = await svc.CreateFolderAsync("", "x");

            // Assert
            r1.Should().BeOfType<Result<System.Collections.Generic.List<DriveItem>, Exception>.Error>();
            r2.Should().BeOfType<Result<System.IO.Stream, Exception>.Error>();
            r3.Should().BeOfType<Result<DriveItem, Exception>.Error>();
            r4.Should().BeOfType<Result<DriveItem, Exception>.Error>();
        }
    }
}
