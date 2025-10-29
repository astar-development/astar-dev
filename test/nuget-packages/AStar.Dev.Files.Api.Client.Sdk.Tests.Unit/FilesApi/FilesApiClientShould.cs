using AStar.Dev.Files.Api.Client.Sdk.Tests.Unit.Helpers;
using AStar.Dev.Files.Api.Client.Sdk.Tests.Unit.MockMessageHandlers;

namespace AStar.Dev.Files.Api.Client.Sdk.Tests.Unit.FilesApi;

public sealed class FilesApiClientShould
{
    [Fact]
    public async Task ReturnExpectedFailureFromGetHealthAsyncWhenTheApIsiUnreachableAsync()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.GetHealthAsync(TestContext.Current.CancellationToken);

        response.Status.ShouldBe("Could not get a response from the AStar.Dev.Files.Api.");
    }

    [Fact]
    public async Task ReturnExpectedFailureMessageFromGetHealthAsyncWhenCheckFailsAsync()
    {
        var sut = FilesApiClientFactory.CreateInternalServerErrorClient("Health Check failed.");

        var response = await sut.GetHealthAsync(TestContext.Current.CancellationToken);

        response.Status.ShouldBe("Health Check failed - Internal Server Error.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromGetHealthAsyncWhenCheckSucceedsAsync()
    {
        var handler = new MockSuccessHttpMessageHandler("");
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.GetHealthAsync(TestContext.Current.CancellationToken);

        response.Status.ShouldBe("OK");
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheCountEndpointAsync()
    {
        var handler = new MockSuccessHttpMessageHandler("Count");
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.GetFilesCountAsync(new());

        response.ShouldBe(0);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheCountEndpointWhenAnErrorOccursAsync()
    {
        var handler = new MockInternalServerErrorHttpMessageHandler("Count");
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.GetFilesCountAsync(new());

        response.ShouldBe(-1);
    }

    [Fact(Skip = "Endpoint not implemented")]
    public async Task ReturnExpectedResponseFromTheCountDuplicatesEndpointAsync()
    {
        const int mockDuplicatesCountValue = 1234;
        var handler = new MockSuccessHttpMessageHandler("CountDuplicates") { Counter = mockDuplicatesCountValue };
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.GetDuplicateFilesCountAsync(new());

        response.Count.ShouldBe(mockDuplicatesCountValue);
    }

    [Fact(Skip = "Endpoint not implemented")]
    public async Task ReturnExpectedResponseFromTheCountDuplicatesEndpointWhenAnErrorOccursAsync()
    {
        var handler = new MockInternalServerErrorHttpMessageHandler("Count");
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.GetDuplicateFilesCountAsync(new());

        response.Count.ShouldBe(-1);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheListEndpointAsync()
    {
        var handler = new MockSuccessHttpMessageHandler("ListFiles");
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.GetFilesAsync(new());

        response.Count().ShouldBe(2);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheListEndpointWhenAnErrorOccursAsync()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut = FilesApiClientFactory.Create(handler);

        Func<Task> sutMethod = async () => await sut.GetFilesAsync(new());

        _ = await sutMethod.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact(Skip = "Endpoint not implemented")]
    public async Task ReturnExpectedResponseFromTheListDuplicatesEndpointAsync()
    {
        var handler = new MockSuccessHttpMessageHandler("ListDuplicates");
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.GetDuplicateFilesAsync(new());

        response.Count.ShouldBe(3);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheListDuplicatesEndpointWhenAnErrorOccursAsync()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut = FilesApiClientFactory.Create(handler);

        Func<Task> sutMethod = async () => await sut.GetDuplicateFilesAsync(new());

        _ = await sutMethod.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForSoftDeletionWhenSuccessfulAsync()
    {
        var handler = new MockSuccessHttpMessageHandler("");
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForSoftDeletionAsync(1);

        response.ShouldBe("Marked for soft deletion");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForSoftDeletionWhenSuccessfulWhenAnErrorOccursAsync()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForSoftDeletionAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForSoftDeletionWhenFailureAsync()
    {
        var sut = FilesApiClientFactory.CreateInternalServerErrorClient("Delete failed...");

        var response = await sut.MarkForSoftDeletionAsync(1);

        response.ShouldBe("Delete failed...");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForSoftDeletionWhenFailureAsync()
    {
        var sut = FilesApiClientFactory.CreateInternalServerErrorClient("Undo mark for deletion failed...");

        var response = await sut.UndoMarkForSoftDeletionAsync(1);

        response.ShouldBe("Undo mark for deletion failed...");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForSoftDeletionWhenSuccessfulAsync()
    {
        var handler = new MockDeletionSuccessHttpMessageHandler();
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForSoftDeletionAsync(1);

        response.ShouldBe("Mark for soft deletion has been undone");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForSoftDeletionWhenAnErrorOccursAsync()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForSoftDeletionAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForHardDeletionWhenSuccessfulAsync()
    {
        var handler = new MockDeletionSuccessHttpMessageHandler();
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForHardDeletionAsync(1);

        response.ShouldBe("Marked for hard deletion.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForHardDeletionWhenAnErrorOccursAsync()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForHardDeletionAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForHardDeletionWhenFailureAsync()
    {
        var sut = FilesApiClientFactory.CreateInternalServerErrorClient("Delete failed...");

        var response = await sut.MarkForHardDeletionAsync(1);

        response.ShouldBe("Delete failed...");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForHardDeletionWhenSuccessfulAsync()
    {
        var handler = new MockSuccessHttpMessageHandler("");
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForHardDeletionAsync(1);

        response.ShouldBe("Mark for hard deletion has been undone");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForHardDeletionWhenAnErrorOccursAsync()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForHardDeletionAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForHardDeletionWhenFailureAsync()
    {
        var sut = FilesApiClientFactory.CreateInternalServerErrorClient("Undo mark for deletion failed...");

        var response = await sut.UndoMarkForHardDeletionAsync(1);

        response.ShouldBe("Undo mark for deletion failed...");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForMovingWhenSuccessfulAsync()
    {
        var handler = new MockDeletionSuccessHttpMessageHandler();
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForMovingAsync(1);

        response.ShouldBe("Mark for moving was successful");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForMovingWhenAnErrorOccursAsync()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.MarkForMovingAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromMarkForMovingWhenFailureAsync()
    {
        var sut = FilesApiClientFactory.CreateInternalServerErrorClient("Delete failed...");

        var response = await sut.MarkForMovingAsync(1);

        response.ShouldBe("Delete failed...");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForMovingWhenSuccessfulAsync()
    {
        var handler = new MockSuccessHttpMessageHandler("");
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForMovingAsync(1);

        response.ShouldBe("Undo mark for moving was successful");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForMovingWhenAnErrorOccursAsync()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.UndoMarkForMovingAsync(1);

        response.ShouldBe("Exception of type 'System.Net.Http.HttpRequestException' was thrown.");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromUndoMarkForMovingWhenFailureAsync()
    {
        var sut = FilesApiClientFactory.CreateInternalServerErrorClient("Undo mark for deletion failed...");

        var response = await sut.UndoMarkForMovingAsync(1);

        response.ShouldBe("Undo mark for deletion failed...");
    }

    [Fact]
    public async Task ReturnExpectedResponseFromThGetFileAccessDetailWhenAnErrorOccursAsync()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut = FilesApiClientFactory.Create(handler);

        Func<Task> sutMethod = async () => await sut.GetFileAccessDetail(1);

        _ = await sutMethod.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact(Skip = "Doesn't work...")]
    public async Task ReturnExpectedResponseFromTheGetFileAccessDetailEndpointAsync()
    {
        var mockFileId = 1;

        var handler = new MockSuccessHttpMessageHandler("FileAccessDetail");
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.GetFileAccessDetail(mockFileId);

        response.Id.ShouldBe(4);
    }

    [Fact]
    public async Task ReturnExpectedResponseFromThGetFileDetailWhenAnErrorOccursAsync()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut = FilesApiClientFactory.Create(handler);

        Func<Task> sutMethod = async () => await sut.GetFileDetail(1);

        _ = await sutMethod.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheGetFileDetailEndpointAsync()
    {
        var handler = new MockSuccessHttpMessageHandler("FileDetail");
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.GetFileDetail(1);

        response.FileName.ShouldBe("Test File FileClassification");
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheUpdateFileAsyncWhenAnErrorOccursAsync()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();
        var sut = FilesApiClientFactory.Create(handler);

        Func<Task> sutMethod = async () => await sut.UpdateFileAsync(new());

        _ = await sutMethod.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task ReturnExpectedResponseFromTheUpdateFileAsyncEndpointAsync()
    {
        var handler = new MockSuccessHttpMessageHandler("FileDetail");
        var sut = FilesApiClientFactory.Create(handler);

        var response = await sut.UpdateFileAsync(new());

        response.ShouldBe("The file details were updated successfully");
    }
}
