using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Graph.Models;
using Xunit;
using AStar.Dev.OneDrive.Client.Services;
// using AStar.Dev.OneDrive.Client.Tests.Unit.Utilities;
using AStar.Dev.OneDrive.Client.Tests.Unit.Fakes;

namespace AStar.Dev.OneDrive.Client.Tests.Unit
{
    public class MainWindowViewModelTests
    {
        [Fact]
        public async Task LoadRootItemsAsync_Populates_RootItems_OnSuccess()
        {
            // Arrange: drive with children
            var child1 = new DriveItem { Id = "1", Name = "a.txt" };
            var child2 = new DriveItem { Id = "2", Name = "b.txt" };
            var drive = new Drive { Id = "drive-id", Root = new DriveItem { Id = "root", Children = new System.Collections.Generic.List<DriveItem> { child1, child2 } } };

            Task<object?> Responder(Microsoft.Kiota.Abstractions.RequestInformation req, Type responseType, CancellationToken ct)
            {
                if (responseType == typeof(Drive)) return Task.FromResult<object?>(drive);
                return Task.FromResult<object?>(null);
            }

            var graphClient = TestUtilities.CreateGraphClient(Responder);
            var login = new FakeLoginService(graphClient);
            var oneDriveService = new OneDriveService(login, NullLogger<OneDriveService>.Instance);
            var vm = new MainWindowViewModel(login, oneDriveService, NullLogger<MainWindowViewModel>.Instance);

            // Act
            await vm.LoadRootCommand.ExecuteAsync(null);

            // Assert
            vm.RootItems.Should().HaveCount(2);
            vm.Status.Should().Contain("Loaded 2");
            vm.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public async Task LoadRootItemsAsync_Sets_Error_OnFailure()
        {
            // Arrange: adapter throws when sending drive request
            Task<object?> Responder(Microsoft.Kiota.Abstractions.RequestInformation req, Type responseType, CancellationToken ct)
            {
                throw new Exception("downstream error");
            }

            var graphClient = TestUtilities.CreateGraphClient(Responder);
            var login = new FakeLoginService(graphClient);
            var oneDriveService = new OneDriveService(login, NullLogger<OneDriveService>.Instance);
            var vm = new MainWindowViewModel(login, oneDriveService, NullLogger<MainWindowViewModel>.Instance);

            // Act
            await vm.LoadRootCommand.ExecuteAsync(null);

            // Assert
            vm.RootItems.Should().HaveCount(0);
            vm.Status.Should().Contain("Load failed");
            vm.ErrorMessage.Should().Contain("downstream error");
        }
    }
}
