using AStar.Dev.Test.Helpers.Minimal.Api;

namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

public class MapGetEndpointShould
{
    [Fact]
    public void ContainTheFilesMapGetEndpoint()
    {
        var builder = new TestEndpointRouteBuilder();

        builder.MapFilesGetEndpoint();

        const string endpointName = "/files";
        builder.ContainsEndpointWithDisplayName(endpointName).ShouldBeTrue();

        builder.GetEndpointMetadataTags(endpointName).ContainsTag("Get Files").ShouldBeTrue();
        builder.GetEndpointMethodData(endpointName).IsGet().ShouldBeTrue();

        builder.GetEndpointResponseTypes(endpointName).DefinesResponseTypeWithStatusCode(200).ShouldBeTrue();
        builder.GetEndpointResponseTypes(endpointName).DefinesResponseTypeWithStatusCode(401).ShouldBeTrue();
        builder.GetEndpointResponseTypes(endpointName).DefinesResponseTypeWithStatusCode(403).ShouldBeTrue();

        builder.GetEndpointResponseTypes("GET /files").DefinesResponseTypeWithType("GetFilesResponse").ShouldBeTrue();
    }
}
