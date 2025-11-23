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

            var adapter = new FakeRequestAdapter(Responder);

            var graphClient = new GraphServiceClient(adapter);

            var login = new FakeLoginService(graphClient);
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
    }
}
