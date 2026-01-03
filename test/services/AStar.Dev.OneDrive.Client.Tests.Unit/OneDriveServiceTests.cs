// using System;
// using System.Collections.Generic;
// using System.Collections.ObjectModel;
// using System.Threading.Tasks;
//
// using Xunit;
// using AStar.Dev.Functional.Extensions;
// using AStar.Dev.OneDrive.Client.Services;
// using AStar.Dev.OneDrive.Client.UserSettings;
// using Microsoft.Graph;
// using Microsoft.Graph.Models;
//
// namespace AStar.Dev.OneDrive.Client.Tests.Unit
// {
//     public sealed class OneDriveServiceTests
//     {
//         private sealed class FailingLoginService : ILoginService
//         {
//             private readonly Exception _ex;
//             public FailingLoginService(Exception ex) => _ex = ex;
//             public Task<Result<GraphServiceClient, Exception>> CreateGraphServiceClientAsync() => Task.FromResult<Result<GraphServiceClient, Exception>>(new Result<GraphServiceClient, Exception>.Error(_ex));
//
//             public Task<Result<bool, Exception>> SignOutAsync(bool hard = false) => Task.FromResult<Result<bool, Exception>>(new Result<bool, Exception>.Ok(true));
//         }
//
//         [Fact]
//         public async Task GetRootItemsAsync_Returns_Error_When_Login_Fails()
//         {
//             var ex = new InvalidOperationException("login failed");
//             var svc = new OneDriveService(new FailingLoginService(ex), new UserPreferences(), Microsoft.Extensions.Logging.Abstractions.NullLogger<OneDriveService>.Instance);
//
//             var result = await svc.RunFullSyncAsync(new(), TestContext.Current.CancellationToken);
//
//             result.ShouldBeOfType<Result<List<DriveItem>, Exception>.Error>();
//             var err = (Result<List<DriveItem>, Exception>.Error)result;
//             err.Reason.Message.ShouldBe("login failed");
//         }
//
//         [Fact]
//         public async Task ApplyToCollectionAsync_Replaces_Collection_OnOk()
//         {
//             // Arrange a successful Result
//             var items = new List<int> { 1, 2, 3 };
//             Task<Result<IEnumerable<int>, Exception>> task = Task.FromResult<Result<IEnumerable<int>, Exception>>(new Result<IEnumerable<int>, Exception>.Ok(items));
//             var coll = new ObservableCollection<int> { 9 };
//
//             // Act
//             await task.ApplyToCollectionAsync(coll);
//
//             // Assert
//             coll.ShouldBeEquivalentTo(items, options => options.WithStrictOrdering());
//         }
//
//         [Fact]
//         public void ToStatus_Returns_ErrorMessage_OnError()
//         {
//             var ex = new Exception("boom");
//             var r = new Result<int, Exception>.Error(ex);
//             var status = r.ToStatus(i => $"ok:{i}");
//             status.ShouldBe("boom");
//         }
//     }
// }
