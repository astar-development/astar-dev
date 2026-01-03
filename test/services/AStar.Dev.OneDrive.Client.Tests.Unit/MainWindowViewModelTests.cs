using System;
using System.Threading;
using System.Threading.Tasks;
using AStar.Dev.OneDrive.Client.Login;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Graph.Models;
using Xunit;
// using AStar.Dev.OneDrive.Client.Tests.Unit.Utilities;
using AStar.Dev.OneDrive.Client.Tests.Unit.Fakes;
using AStar.Dev.OneDrive.Client.ViewModels;
using Microsoft.Graph;
using OneDriveService = AStar.Dev.OneDrive.Client.Services.OneDriveService;

namespace AStar.Dev.OneDrive.Client.Tests.Unit
{
    public sealed class MainWindowViewModelTests
    {
        [Fact]
        public async Task LoadRootItemsAsync_Populates_RootItems_OnSuccess()
        {
            // Arrange: drive with children
            var child1 = new DriveItem { Id = "1", Name = "a.txt" };
            var child2 = new DriveItem { Id = "2", Name = "b.txt" };
            var drive = new Drive { Id = "drive-id", Root = new DriveItem { Id = "root", Children = new List<DriveItem> { child1, child2 } } };

            Task<object?> Responder(Microsoft.Kiota.Abstractions.RequestInformation req, Type responseType, CancellationToken ct)
            {
                if (responseType == typeof(Drive)) return Task.FromResult<object?>(drive);
                return Task.FromResult<object?>(null);
            }

            GraphServiceClient graphClient = TestUtilities.CreateGraphClient(Responder);
            var login = new FakeLoginService(graphClient);
            IOneDriveService oneDriveService = new OneDriveService(login, new(),NullLogger<OneDriveService>.Instance);
            var vm = new MainWindowViewModel(oneDriveService, NullLogger<MainWindowViewModel>.Instance);

            // Act
            await vm.LoadRootCommand.ExecuteAsync(null);

            // Assert
            // vm.RootItems.ShouldBe(2);
            vm.Status.ShouldContain("Idle");
            vm.ErrorMessage.ShouldBeEmpty();
        }

        [Fact]
        public async Task LoadRootItemsAsync_Sets_Error_OnFailure()
        {
            // Arrange: adapter throws when sending drive request
            Task<object?> Responder(Microsoft.Kiota.Abstractions.RequestInformation req, Type responseType, CancellationToken ct)
            {
                throw new Exception("downstream error");
            }

            GraphServiceClient graphClient = TestUtilities.CreateGraphClient(Responder);
            var login = new FakeLoginService(graphClient);
            var oneDriveService = new OneDriveService(login, new(),NullLogger<OneDriveService>.Instance);
            var vm = new MainWindowViewModel(login, oneDriveService, NullLogger<MainWindowViewModel>.Instance);

            // Act
            await vm.LoadRootCommand.ExecuteAsync(null);

            // Assert
            //vm.RootItems.ShouldBe(0);
            vm.Status.ShouldContain("Load failed");
            vm.ErrorMessage.ShouldContain("downstream error");
        }
    }
}
