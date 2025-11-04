using AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;
using Microsoft.AspNetCore.Http;

namespace AStar.Dev.Files.Classifications.Api.Tests.Unit.Endpoints.FileClassifications.V1;

public class GetFileClassificationRequestTests
{
    [Fact]
    public void Defaults_ShouldBeCorrect()
    {
        var request = new GetFileClassificationRequest();

        request.CurrentPage.ShouldBe(1);
        request.ItemsPerPage.ShouldBe(10);
    }

    [Fact]
    public void Can_Override_Values()
    {
        var request = new GetFileClassificationRequest(3, 25);

        request.CurrentPage.ShouldBe(3);
        request.ItemsPerPage.ShouldBe(25);
    }

    [Fact]
    public void Name_Should_Match_EndpointConstants_GroupName()
    {
        var request = new GetFileClassificationRequest();

        request.Name.ShouldBe(EndpointConstants.GetFileClassificationsGroupName);
    }

    [Fact]
    public void HttpMethod_Should_Be_Get()
    {
        var request = new GetFileClassificationRequest();

        request.HttpMethod.ShouldBe(HttpMethods.Get);
    }
}
