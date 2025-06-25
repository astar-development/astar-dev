using AStar.Dev.Images.Api.Client.SDK.ImagesApi;
using AStar.Dev.Images.Api.Client.Sdk.MockMessageHandlers;
using Microsoft.Extensions.Logging.Abstractions;

namespace AStar.Dev.Images.Api.Client.Sdk;

public sealed class ImagesApiClientShould

{
    [Fact]
    public async Task ReturnExpectedFailureFromGetHealthAsyncWhenTheApIsiUnreachable()
    {
        var handler = new MockHttpRequestExceptionErrorHttpMessageHandler();

        var httpClient = new HttpClient(handler) { BaseAddress = new("https://doesnot.matter.com"), };

        var sut = new ImagesApiClient(httpClient, NullLogger<ImagesApiClient>.Instance);

        var response = await sut.GetHealthAsync();

        response.Status.ShouldBe("Could not get a response from the AStar.Dev.Images.Api");
    }

    [Fact]
    public async Task ReturnExpectedFailureMessageFromGetHealthAsyncWhenCheckFails()
    {
        var handler = new MockInternalServerErrorHttpMessageHandler("Health Check failed - Internal Server Error.");

        var httpClient = new HttpClient(handler) { BaseAddress = new("https://doesnot.matter.com"), };

        var sut = new ImagesApiClient(httpClient, NullLogger<ImagesApiClient>.Instance);

        var response = await sut.GetHealthAsync();

        response.Status.ShouldBe("Health Check failed - Internal Server Error");
    }

    [Fact]
    public async Task ReturnExpectedMessageFromGetHealthAsyncWhenCheckSucceeds()
    {
        var handler = new MockSuccessHttpMessageHandler("");

        var httpClient = new HttpClient(handler) { BaseAddress = new("https://doesnot.matter.com"), };

        var sut = new ImagesApiClient(httpClient, NullLogger<ImagesApiClient>.Instance);

        var response = await sut.GetHealthAsync();

        response.Status.ShouldBe("OK");
    }
}
