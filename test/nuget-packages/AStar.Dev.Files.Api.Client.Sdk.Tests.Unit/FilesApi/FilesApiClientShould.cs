using AStar.Dev.Api.HealthChecks;
using AStar.Dev.Files.Api.Client.SDK.FilesApi;
using AStar.Dev.Files.Api.Client.Sdk.Helpers;
using AStar.Dev.Files.Api.Client.Sdk.MockMessageHandlers;
using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.FilesApi;

public sealed class FilesApiClientShould
{
    [Fact]
    public async Task ReturnExpectedFailureFromGetHealthAsyncWhenTheApIsiUnreachable()
    {
        var            handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        HealthStatusResponse response = await sut.GetHealthAsync(TestContext.Current.CancellationToken);

        response.Status.ShouldBe("Could not get a response from the AStar.Dev.Files.Api.");
    }

    [Fact]
    public async Task ReturnExpectedFailureMessageFromGetHealthAsyncWhenCheckFails()
    {
        FilesApiClient sut = FilesApiClientFactory.CreateInternalServerErrorClient("Health Check failed.");

        HealthStatusResponse response = await sut.GetHealthAsync(TestContext.Current.CancellationToken);

        response.Status.ShouldBe("Health Check failed - Internal Server Error.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromGetHealthAsyncWhenCheckSucceeds()
    {
        var            handler = new MockSuccessHttpMessageHandler("");
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        HealthStatusResponse response = await sut.GetHealthAsync(TestContext.Current.CancellationToken);

        response.Status.ShouldBe("OK");
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheCountEndpoint()
    {
        var            handler = new MockSuccessHttpMessageHandler("Count");
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.GetFilesCountAsync(new SearchParameters());

        response.ShouldBe(0);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheCountEndpointWhenAnErrorOccurs()
    {
        var            handler = new MockInternalServerErrorHttpMessageHandler("Count");
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.GetFilesCountAsync(new SearchParameters());

        response.ShouldBe(-1);
    }

    [Fact(Skip = "Mock doesn't work")]
    public async Task ReturnExpectedResponseFromTheCountDuplicatesEndpoint()
    {
        const int      mockDuplicatesCountValue = 1234;
        var            handler                  = new MockSuccessHttpMessageHandler("CountDuplicates") { Counter = mockDuplicatesCountValue, };
        FilesApiClient sut                      = FilesApiClientFactory.Create(handler);

        GetDuplicatesCountQueryResponse response = await sut.GetDuplicateFilesCountAsync(new SearchParameters());

        response.Count.ShouldBe(mockDuplicatesCountValue);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheCountDuplicatesEndpointWhenAnErrorOccurs()
    {
        var            handler = new MockInternalServerErrorHttpMessageHandler("Count");
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        GetDuplicatesCountQueryResponse response = await sut.GetDuplicateFilesCountAsync(new SearchParameters());

        response.Count.ShouldBe(-1);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheListEndpoint()
    {
        var            handler = new MockSuccessHttpMessageHandler("ListFiles");
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        IEnumerable<FileDetail> response = await sut.GetFilesAsync(new SearchParameters());

        response.Count().ShouldBe(2);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheListEndpointWhenAnErrorOccurs()
    {
        var            handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        Func<Task> sutMethod = async () => await sut.GetFilesAsync(new SearchParameters());

        await sutMethod.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact(Skip = "Mock doesn't work")]
    public async Task ReturnExpectedResponseFromTheListDuplicatesEndpoint()
    {
        var            handler = new MockSuccessHttpMessageHandler("ListDuplicates");
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        IReadOnlyCollection<DuplicatesList> response = await sut.GetDuplicateFilesAsync(new SearchParameters());

        response.Count.ShouldBe(3);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheListDuplicatesEndpointWhenAnErrorOccurs()
    {
        var            handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        Func<Task> sutMethod = async () => await sut.GetDuplicateFilesAsync(new SearchParameters());

        await sutMethod.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForSoftDeletionWhenSuccessful()
    {
        var            handler = new MockSuccessHttpMessageHandler("");
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForSoftDeletionAsync(1);

        response.ShouldBe("Marked for soft deletion");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForSoftDeletionWhenSuccessfulWhenAnErrorOccurs()
    {
        var            handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForSoftDeletionAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForSoftDeletionWhenFailure()
    {
        FilesApiClient sut = FilesApiClientFactory.CreateInternalServerErrorClient("Delete failed...");

        var response = await sut.MarkForSoftDeletionAsync(1);

        response.ShouldBe("Delete failed...");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForSoftDeletionWhenFailure()
    {
        FilesApiClient sut = FilesApiClientFactory.CreateInternalServerErrorClient("Undo mark for deletion failed...");

        var response = await sut.UndoMarkForSoftDeletionAsync(1);

        response.ShouldBe("Undo mark for deletion failed...");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForSoftDeletionWhenSuccessful()
    {
        var            handler = new MockDeletionSuccessHttpMessageHandler();
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForSoftDeletionAsync(1);

        response.ShouldBe("Mark for soft deletion has been undone");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForSoftDeletionWhenAnErrorOccurs()
    {
        var            handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForSoftDeletionAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForHardDeletionWhenSuccessful()
    {
        var            handler = new MockDeletionSuccessHttpMessageHandler();
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForHardDeletionAsync(1);

        response.ShouldBe("Marked for hard deletion.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForHardDeletionWhenAnErrorOccurs()
    {
        var            handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForHardDeletionAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForHardDeletionWhenFailure()
    {
        FilesApiClient sut = FilesApiClientFactory.CreateInternalServerErrorClient("Delete failed...");

        var response = await sut.MarkForHardDeletionAsync(1);

        response.ShouldBe("Delete failed...");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForHardDeletionWhenSuccessful()
    {
        var            handler = new MockSuccessHttpMessageHandler("");
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForHardDeletionAsync(1);

        response.ShouldBe("Mark for hard deletion has been undone");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForHardDeletionWhenAnErrorOccurs()
    {
        var            handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForHardDeletionAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForHardDeletionWhenFailure()
    {
        FilesApiClient sut = FilesApiClientFactory.CreateInternalServerErrorClient("Undo mark for deletion failed...");

        var response = await sut.UndoMarkForHardDeletionAsync(1);

        response.ShouldBe("Undo mark for deletion failed...");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForMovingWhenSuccessful()
    {
        var            handler = new MockDeletionSuccessHttpMessageHandler();
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForMovingAsync(1);

        response.ShouldBe("Mark for moving was successful");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForMovingWhenAnErrorOccurs()
    {
        var            handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForMovingAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForMovingWhenFailure()
    {
        FilesApiClient sut = FilesApiClientFactory.CreateInternalServerErrorClient("Delete failed...");

        var response = await sut.MarkForMovingAsync(1);

        response.ShouldBe("Delete failed...");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForMovingWhenSuccessful()
    {
        var            handler = new MockSuccessHttpMessageHandler("");
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForMovingAsync(1);

        response.ShouldBe("Undo mark for moving was successful");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForMovingWhenAnErrorOccurs()
    {
        var            handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForMovingAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForMovingWhenFailure()
    {
        FilesApiClient sut = FilesApiClientFactory.CreateInternalServerErrorClient("Undo mark for deletion failed...");

        var response = await sut.UndoMarkForMovingAsync(1);

        response.ShouldBe("Undo mark for deletion failed...");
    }

    [Fact]
    public async Task ReturnExpectedResponseFromThGetFileAccessDetailWhenAnErrorOccurs()
    {
        var            handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        Func<Task> sutMethod = async () => await sut.GetFileAccessDetail(1);

        await sutMethod.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact(Skip = "Doesn't work...")]
    public async Task ReturnExpectedResponseFromTheGetFileAccessDetailEndpoint()
    {
        var mockFileId = 1;

        var            handler = new MockSuccessHttpMessageHandler("FileAccessDetail");
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        FileAccessDetail response = await sut.GetFileAccessDetail(mockFileId);

        response.Id.ShouldBe(4);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromThGetFileDetailWhenAnErrorOccurs()
    {
        var            handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        Func<Task> sutMethod = async () => await sut.GetFileDetail(1);

        await sutMethod.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheGetFileDetailEndpoint()
    {
        var            handler = new MockSuccessHttpMessageHandler("FileDetail");
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        FileDetail response = await sut.GetFileDetail(1);

        response.FileName.ShouldBe("Test File FileClassification");
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheUpdateFileAsyncWhenAnErrorOccurs()
    {
        var            handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        Func<Task> sutMethod = async () => await sut.UpdateFileAsync(new DirectoryChangeRequest());

        await sutMethod.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheUpdateFileAsyncEndpoint()
    {
        var            handler = new MockSuccessHttpMessageHandler("FileDetail");
        FilesApiClient sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.UpdateFileAsync(new DirectoryChangeRequest());

        response.ShouldBe("The file details were updated successfully");
    }
}
