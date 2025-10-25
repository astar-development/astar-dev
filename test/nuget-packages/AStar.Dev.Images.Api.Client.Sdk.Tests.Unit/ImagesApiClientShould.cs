using AStar.Dev.Images.Api.Client.SDK.ImagesApi;
using AStar.Dev.Images.Api.Client.Sdk.Tests.Unit.MockMessageHandlers;
using Microsoft.Extensions.Logging.Abstractions;

namespace AStar.Dev.Images.Api.Client.Sdk.Tests.Unit;

public sealed class ImagesApiClientShould

{
    [Fact]
    public async Task ReturnExpectedFailureFromGetHealthAsyncWhenTheApIsiUnreachableAsync()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();

        var httpClient = new HttpClient(handler) { BaseAddress = new("https://doesnot.matter.com") };

        var sut = new ImagesApiClient(httpClient, NullLogger<ImagesApiClient>.Instance);

        var response = await sut.GetHealthAsync(TestContext.Current.CancellationToken);

        response.Status.ShouldBe("Could not get a response from the AStar.Dev.Images.Api");
    }

    [Fact]
    public async Task ReturnExpectedFailureMessageFromGetHealthAsyncWhenCheckFailsAsync()
    {
        var handler = new MockInternalServerErrorHttpMessageHandler("Health Check failed - Internal Server Error.");

        var httpClient = new HttpClient(handler) { BaseAddress = new("https://doesnot.matter.com") };

        var sut = new ImagesApiClient(httpClient, NullLogger<ImagesApiClient>.Instance);

        var response = await sut.GetHealthAsync(TestContext.Current.CancellationToken);

        response.Status.ShouldBe("Health Check failed - Internal Server Error");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromGetHealthAsyncWhenCheckSucceedsAsync()
    {
        var handler = new MockSuccessHttpMessageHandler("");

        var httpClient = new HttpClient(handler) { BaseAddress = new("https://doesnot.matter.com") };

        var sut = new ImagesApiClient(httpClient, NullLogger<ImagesApiClient>.Instance);

        var response = await sut.GetHealthAsync(TestContext.Current.CancellationToken);

        response.Status.ShouldBe("OK");
    }
}
