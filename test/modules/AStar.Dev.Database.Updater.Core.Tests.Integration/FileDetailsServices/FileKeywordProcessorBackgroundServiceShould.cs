using System.Reflection;
using AStar.Dev.Database.Updater.Core.FileDetailsServices;
using AStar.Dev.Functional.Extensions;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace AStar.Dev.Database.Updater.Core.Tests.Integration.FileDetailsServices;

#pragma warning disable xUnit1051

public class FileKeywordProcessorBackgroundServiceShould
{
    [Fact]
    public async Task ExecuteAsyncWhenRunNewFilesServiceAndFileListOkCallsProcessAsync()
    {
        // Arrange
        var config = new DatabaseUpdaterConfiguration
        {
            RunNewFilesService = true,
            RootDirectory = "root",
            HonourFirstDelay = false,
            MappingsFilePath = string.Empty,
            SoftDeleteScheduledTime = TimeOnly.FromTimeSpan(TimeSpan.Zero),
            HardDeleteScheduledTime = TimeOnly.FromTimeSpan(TimeSpan.Zero),
            NewFilesScheduledTime = TimeOnly.FromTimeSpan(TimeSpan.Zero)
        };
        var options = Options.Create(config);

        var fileList = new List<FileDetail>
        {
            new ()
            {
                Id = new FileId { Value = Guid.NewGuid() },
                FileName = new FileName("a.txt"),
                DirectoryName = new DirectoryName("/tmp"),
                FileSize = 0,
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow
            }
        };

        var resultOk = new Result<List<FileDetail>, ErrorResponse>.Ok(fileList);

        // Build a test FilesContext using an in-memory provider and real FilesProcessor
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        _ = optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        var filesContext = new FilesContext(optionsBuilder.Options);

        var keywordProvider = Substitute.For<IKeywordProvider>();
        _ = keywordProvider.GetKeywordsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IReadOnlyList<FileNamePartsWithClassifications>>(new List<FileNamePartsWithClassifications>()));

        // Use substitutes for the list service and files processor to avoid EF projection in FileListService.Get
        var fileListServiceFake = Substitute.For<IFileListService>();
        _ = fileListServiceFake.Get(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<List<FileDetail>, ErrorResponse>>(resultOk));

        var filesProcessorFake = Substitute.For<IFilesProcessor>();
#pragma warning disable CS4014
        _ = filesProcessorFake.ProcessAsync(Arg.Any<IReadOnlyCollection<FileDetail>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<bool, ErrorResponse>>(new Result<bool, ErrorResponse>.Ok(true)));
#pragma warning restore CS4014

        var logger = Substitute.For<ILogger<FileKeywordProcessorBackgroundService>>();

        var providerForService = Substitute.For<IServiceProvider>();
        _ = providerForService.GetService(typeof(IOptions<DatabaseUpdaterConfiguration>)).Returns(options);
        _ = providerForService.GetService(typeof(IFilesProcessor)).Returns(filesProcessorFake);
        _ = providerForService.GetService(typeof(IFileListService)).Returns(fileListServiceFake);
        _ = providerForService.GetService(typeof(ILogger<FileKeywordProcessorBackgroundService>)).Returns(logger);

        var scopeForService = Substitute.For<IServiceScope>();
        _ = scopeForService.ServiceProvider.Returns(providerForService);

        var factoryForService = Substitute.For<IServiceScopeFactory>();
        _ = factoryForService.CreateScope().Returns(scopeForService);

        var svc = new FileKeywordProcessorBackgroundService(factoryForService);

        // Act - ExecuteAsync is protected; invoke via reflection
        var execute = svc.GetType().GetMethod("ExecuteAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var task    = (Task)execute.Invoke(svc, [TestContext.Current.CancellationToken])!;
        await task;

        // Assert: files processor was invoked with the list returned by the file list service
        await Task.Yield(); // ensure async call completed
#pragma warning disable CS4014
        _ = filesProcessorFake.Received(1).ProcessAsync(Arg.Is<IReadOnlyCollection<FileDetail>>(c => c.Count == 1 && c.First().FileName.Value == "a.txt"), Arg.Any<CancellationToken>());
#pragma warning restore CS4014
    }

    [Fact]
    public async Task ExecuteAsyncWhenFileListReturnsErrorLogsAndDoesNotCallProcessAsync()
    {
        // Arrange
        var config = new DatabaseUpdaterConfiguration
        {
            RunNewFilesService = true,
            RootDirectory = "root",
            HonourFirstDelay = false,
            MappingsFilePath = string.Empty,
            SoftDeleteScheduledTime = TimeOnly.FromTimeSpan(TimeSpan.Zero),
            HardDeleteScheduledTime = TimeOnly.FromTimeSpan(TimeSpan.Zero),
            NewFilesScheduledTime = TimeOnly.FromTimeSpan(TimeSpan.Zero)
        };
        var options = Options.Create(config);

        var resultError = new Result<List<FileDetail>, ErrorResponse>.Error(new ErrorResponse("oops"));

        // For error scenario: use an empty MockFileSystem and path that will cause enumeration to throw
        var optionsBuilder2 = new DbContextOptionsBuilder<FilesContext>();
        _ = optionsBuilder2.UseInMemoryDatabase(Guid.NewGuid().ToString());

        _ = new FilesContext(optionsBuilder2.Options);

        var keywordProvider2 = Substitute.For<IKeywordProvider>();
        _ = keywordProvider2.GetKeywordsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IReadOnlyList<FileNamePartsWithClassifications>>(new List<FileNamePartsWithClassifications>()));

        var fileListServiceFake2 = Substitute.For<IFileListService>();
        _ = fileListServiceFake2.Get(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<List<FileDetail>, ErrorResponse>>(resultError));

        var filesProcessorFake2 = Substitute.For<IFilesProcessor>();
#pragma warning disable CS4014
        _ = filesProcessorFake2.ProcessAsync(Arg.Any<IReadOnlyCollection<FileDetail>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<bool, ErrorResponse>>(new Result<bool, ErrorResponse>.Ok(true)));
#pragma warning restore CS4014

        var providerForService2 = Substitute.For<IServiceProvider>();
        _ = providerForService2.GetService(typeof(IOptions<DatabaseUpdaterConfiguration>)).Returns(options);
        _ = providerForService2.GetService(typeof(IFilesProcessor)).Returns(filesProcessorFake2);
        _ = providerForService2.GetService(typeof(IFileListService)).Returns(fileListServiceFake2);
        _ = providerForService2.GetService(typeof(ILogger<FileKeywordProcessorBackgroundService>)).Returns(Substitute.For<ILogger<FileKeywordProcessorBackgroundService>>());

        var scopeForService2 = Substitute.For<IServiceScope>();
        _ = scopeForService2.ServiceProvider.Returns(providerForService2);

        var factoryForService2 = Substitute.For<IServiceScopeFactory>();
        _ = factoryForService2.CreateScope().Returns(scopeForService2);

        var svc2 = new FileKeywordProcessorBackgroundService(factoryForService2);

        // Act - protected method: call via reflection
        var execute2 = svc2.GetType().GetMethod("ExecuteAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var task2    = (Task)execute2.Invoke(svc2, [TestContext.Current.CancellationToken])!;
        await task2;

        // Assert: files processor was not called because file list returned an error
#pragma warning disable CS4014
        _ = filesProcessorFake2.DidNotReceiveWithAnyArgs().ProcessAsync(default!, default);
#pragma warning restore CS4014
    }
}
