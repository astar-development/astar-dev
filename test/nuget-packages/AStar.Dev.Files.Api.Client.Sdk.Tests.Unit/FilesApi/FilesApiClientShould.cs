using AStar.Dev.Files.Api.Client.Sdk.Helpers;
using AStar.Dev.Files.Api.Client.Sdk.MockMessageHandlers;

namespace AStar.Dev.Files.Api.Client.Sdk.FilesApi;

public sealed class FilesApiClientShould
{
    [Fact]
    public async Task ReturnExpectedFailureFromGetHealthAsyncWhenTheApIsiUnreachable()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.GetHealthAsync();

        response.Status.ShouldBe("Could not get a response from the AStar.Dev.Files.Api.");
    }

    [Fact]
    public async Task ReturnExpectedFailureMessageFromGetHealthAsyncWhenCheckFails()
    {
        var sut = FilesApiClientFactory.CreateInternalServerErrorClient("Health Check failed.");

        var response = await sut.GetHealthAsync();

        response.Status.ShouldBe("Health Check failed - Internal Server Error.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromGetHealthAsyncWhenCheckSucceeds()
    {
        var handler = new MockSuccessHttpMessageHandler("");
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.GetHealthAsync();

        response.Status.ShouldBe("OK");
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheCountEndpoint()
    {
        var handler = new MockSuccessHttpMessageHandler("Count");
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.GetFilesCountAsync(new());

        response.ShouldBe(0);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheCountEndpointWhenAnErrorOccurs()
    {
        var handler = new MockInternalServerErrorHttpMessageHandler("Count");
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.GetFilesCountAsync(new());

        response.ShouldBe(-1);
    }

    [Fact(Skip = "Mock doesn't work")]
    public async Task ReturnExpectedResponseFromTheCountDuplicatesEndpoint()
    {
        const int mockDuplicatesCountValue = 1234;
        var       handler                  = new MockSuccessHttpMessageHandler("CountDuplicates") { Counter = mockDuplicatesCountValue };
        var       sut                      = FilesApiClientFactory.Create(handler);

        var response = await sut.GetDuplicateFilesCountAsync(new());

        response.Count.ShouldBe(mockDuplicatesCountValue);
    }

    [Fact(Skip = "Doesn't work")]
    public async Task ReturnExpectedResponseFromTheCountDuplicatesEndpointWhenAnErrorOccurs()
    {
        var handler = new MockInternalServerErrorHttpMessageHandler("Count");
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.GetDuplicateFilesCountAsync(new());

        response.Count.ShouldBe(-1);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheListEndpoint()
    {
        var handler = new MockSuccessHttpMessageHandler("ListFiles");
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.GetFilesAsync(new());

        response.Count().ShouldBe(2);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheListEndpointWhenAnErrorOccurs()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut     = FilesApiClientFactory.Create(handler);

        Func<Task> sutMethod = async () => await sut.GetFilesAsync(new());

        await sutMethod.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact(Skip = "Mock doesn't work")]
    public async Task ReturnExpectedResponseFromTheListDuplicatesEndpoint()
    {
        var handler = new MockSuccessHttpMessageHandler("ListDuplicates");
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.GetDuplicateFilesAsync(new());

        response.Count.ShouldBe(3);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheListDuplicatesEndpointWhenAnErrorOccurs()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut     = FilesApiClientFactory.Create(handler);

        Func<Task> sutMethod = async () => await sut.GetDuplicateFilesAsync(new());

        await sutMethod.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForSoftDeletionWhenSuccessful()
    {
        var handler = new MockSuccessHttpMessageHandler("");
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForSoftDeletionAsync(1);

        response.ShouldBe("Marked for soft deletion");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForSoftDeletionWhenSuccessfulWhenAnErrorOccurs()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForSoftDeletionAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForSoftDeletionWhenFailure()
    {
        var sut = FilesApiClientFactory.CreateInternalServerErrorClient("Delete failed...");

        var response = await sut.MarkForSoftDeletionAsync(1);

        response.ShouldBe("Delete failed...");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForSoftDeletionWhenFailure()
    {
        var sut = FilesApiClientFactory.CreateInternalServerErrorClient("Undo mark for deletion failed...");

        var response = await sut.UndoMarkForSoftDeletionAsync(1);

        response.ShouldBe("Undo mark for deletion failed...");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForSoftDeletionWhenSuccessful()
    {
        var handler = new MockDeletionSuccessHttpMessageHandler();
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForSoftDeletionAsync(1);

        response.ShouldBe("Mark for soft deletion has been undone");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForSoftDeletionWhenAnErrorOccurs()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForSoftDeletionAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForHardDeletionWhenSuccessful()
    {
        var handler = new MockDeletionSuccessHttpMessageHandler();
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForHardDeletionAsync(1);

        response.ShouldBe("Marked for hard deletion.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForHardDeletionWhenAnErrorOccurs()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForHardDeletionAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForHardDeletionWhenFailure()
    {
        var sut = FilesApiClientFactory.CreateInternalServerErrorClient("Delete failed...");

        var response = await sut.MarkForHardDeletionAsync(1);

        response.ShouldBe("Delete failed...");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForHardDeletionWhenSuccessful()
    {
        var handler = new MockSuccessHttpMessageHandler("");
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForHardDeletionAsync(1);

        response.ShouldBe("Mark for hard deletion has been undone");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForHardDeletionWhenAnErrorOccurs()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForHardDeletionAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForHardDeletionWhenFailure()
    {
        var sut = FilesApiClientFactory.CreateInternalServerErrorClient("Undo mark for deletion failed...");

        var response = await sut.UndoMarkForHardDeletionAsync(1);

        response.ShouldBe("Undo mark for deletion failed...");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForMovingWhenSuccessful()
    {
        var handler = new MockDeletionSuccessHttpMessageHandler();
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForMovingAsync(1);

        response.ShouldBe("Mark for moving was successful");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForMovingWhenAnErrorOccurs()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForMovingAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForMovingWhenFailure()
    {
        var sut = FilesApiClientFactory.CreateInternalServerErrorClient("Delete failed...");

        var response = await sut.MarkForMovingAsync(1);

        response.ShouldBe("Delete failed...");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForMovingWhenSuccessful()
    {
        var handler = new MockSuccessHttpMessageHandler("");
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForMovingAsync(1);

        response.ShouldBe("Undo mark for moving was successful");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForMovingWhenAnErrorOccurs()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForMovingAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForMovingWhenFailure()
    {
        var sut = FilesApiClientFactory.CreateInternalServerErrorClient("Undo mark for deletion failed...");

        var response = await sut.UndoMarkForMovingAsync(1);

        response.ShouldBe("Undo mark for deletion failed...");
    }

    [Fact]
    public async Task ReturnExpectedResponseFromThGetFileAccessDetailWhenAnErrorOccurs()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut     = FilesApiClientFactory.Create(handler);

        Func<Task> sutMethod = async () => await sut.GetFileAccessDetail(1);

        await sutMethod.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact(Skip = "Doesn't work...")]
    public async Task ReturnExpectedResponseFromTheGetFileAccessDetailEndpoint()
    {
        var mockFileId = 1;

        var handler = new MockSuccessHttpMessageHandler("FileAccessDetail");
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.GetFileAccessDetail(mockFileId);

        response.Id.ShouldBe(4);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromThGetFileDetailWhenAnErrorOccurs()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut     = FilesApiClientFactory.Create(handler);

        Func<Task> sutMethod = async () => await sut.GetFileDetail(1);

        await sutMethod.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheGetFileDetailEndpoint()
    {
        var handler = new MockSuccessHttpMessageHandler("FileDetail");
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.GetFileDetail(1);

        response.FileName.ShouldBe("Test File FileClassification");
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheUpdateFileAsyncWhenAnErrorOccurs()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut     = FilesApiClientFactory.Create(handler);

        Func<Task> sutMethod = async () => await sut.UpdateFileAsync(new());

        await sutMethod.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheUpdateFileAsyncEndpoint()
    {
        var handler = new MockSuccessHttpMessageHandler("FileDetail");
        var sut     = FilesApiClientFactory.Create(handler);

        var response = await sut.UpdateFileAsync(new());

        response.ShouldBe("The file details were updated successfully");
    }
}
