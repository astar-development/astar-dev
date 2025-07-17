using AStar.Dev.Test.Helpers.Minimal.Api;
using JetBrains.Annotations;

namespace AStar.Dev.Files.Api.Endpoints.Add.V1;

[TestSubject(typeof(MapPostEndpoint))]
public class MapPostEndpointShould
{
    [Fact]
    public void ContainMapFilesPostEndpoint()
    {
        var builder = new TestEndpointRouteBuilder();

        builder.MapFilesPostEndpoint();

        const string endpointName = "/files";
        builder.ContainsEndpointWithDisplayName(endpointName).ShouldBeTrue();

        builder.GetEndpointMetadataTags(endpointName).ContainsTag("Add Files").ShouldBeTrue();
        builder.GetEndpointMethodData(endpointName).IsPost().ShouldBeTrue();

        builder.GetEndpointResponseTypes(endpointName).DefinesResponseTypeWithStatusCode(201).ShouldBeTrue();
        builder.GetEndpointResponseTypes(endpointName).DefinesResponseTypeWithStatusCode(401).ShouldBeTrue();
        builder.GetEndpointResponseTypes(endpointName).DefinesResponseTypeWithStatusCode(403).ShouldBeTrue();

        builder.GetEndpointResponseTypes("POST /files").DefinesResponseTypeWithType("AddFilesResponse").ShouldBeTrue();
    }
}
