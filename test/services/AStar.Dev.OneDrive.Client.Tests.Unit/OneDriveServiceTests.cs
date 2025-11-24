using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using AStar.Dev.Functional.Extensions;
using AStar.Dev.OneDrive.Client.Services;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace AStar.Dev.OneDrive.Client.Tests.Unit
{
    public sealed class OneDriveServiceTests
    {
        private sealed class FailingLoginService : ILoginService
        {
            private readonly Exception _ex;
            public FailingLoginService(Exception ex) => _ex = ex;
            public bool IsSignedIn => false;
            public Task<Result<GraphServiceClient, Exception>> SignInAsync() => Task.FromResult<Result<GraphServiceClient, Exception>>(new Result<GraphServiceClient, Exception>.Error(_ex));
            public Task<Result<bool, Exception>> SignOutAsync() => Task.FromResult<Result<bool, Exception>>(new Result<bool, Exception>.Ok(true));
        }

        [Fact]
        public async Task GetRootItemsAsync_Returns_Error_When_Login_Fails()
        {
            var ex = new InvalidOperationException("login failed");
            var svc = new OneDriveService(new FailingLoginService(ex), Microsoft.Extensions.Logging.Abstractions.NullLogger<OneDriveService>.Instance);

            var result = await svc.GetRootItemsAsync();

            result.Should().BeOfType<Result<List<DriveItem>, Exception>.Error>();
            var err = (Result<List<DriveItem>, Exception>.Error)result;
            err.Reason.Message.Should().Be("login failed");
        }

        [Fact]
        public async Task ApplyToCollectionAsync_Replaces_Collection_OnOk()
        {
            // Arrange a successful Result
            var items = new List<int> { 1, 2, 3 };
            Task<Result<IEnumerable<int>, Exception>> task = Task.FromResult<Result<IEnumerable<int>, Exception>>(new Result<IEnumerable<int>, Exception>.Ok(items));
            var coll = new ObservableCollection<int> { 9 };

            // Act
            await task.ApplyToCollectionAsync(coll);

            // Assert
            coll.Should().BeEquivalentTo(items, options => options.WithStrictOrdering());
        }

        [Fact]
        public void ToStatus_Returns_ErrorMessage_OnError()
        {
            var ex = new Exception("boom");
            var r = new Result<int, Exception>.Error(ex);
            var status = r.ToStatus(i => $"ok:{i}");
            status.Should().Be("boom");
        }
    }
}
