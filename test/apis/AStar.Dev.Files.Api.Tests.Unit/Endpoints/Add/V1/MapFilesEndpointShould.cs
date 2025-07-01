using AStar.Dev.Test.Helpers.Minimal.Api;
using JetBrains.Annotations;

namespace AStar.Dev.Files.Api.Endpoints.Add.V1;

[TestSubject(typeof(MapPostEndpoint))]
public class MapFilesEndpointShould
{
    [Fact]
    public void ContainTheMapFilesEndpoint()
    {
        var builder = new TestEndpointRouteBuilder();

        builder.MapFilesPostEndpoint();

        const string endpointName = "/files";
        builder.ContainsEndpointWithDisplayName(endpointName).ShouldBeTrue();

        builder.GetEndpointMetadataTags(endpointName).ContainsTag("Add Files").ShouldBeTrue();
        builder.GetEndpointMethodData(endpointName).IsPost().ShouldBeTrue();

        // interestingly, Swagger doesn't list 200 as soon as we add 201 etc... not sure how / why this passes - need to look into it more, I guess. Initial look doesn't reveal the cause
        // builder.GetEndpointResponseTypes(endpointName).DefinesResponseTypeWithStatusCode(200).ShouldBeTrue();
        builder.GetEndpointResponseTypes(endpointName).DefinesResponseTypeWithStatusCode(201).ShouldBeTrue();
        builder.GetEndpointResponseTypes(endpointName).DefinesResponseTypeWithStatusCode(401).ShouldBeTrue();
        builder.GetEndpointResponseTypes(endpointName).DefinesResponseTypeWithStatusCode(403).ShouldBeTrue();

        //builder.GetEndpointResponseTypes("POST /files").DefinesResponseTypeWithType("dunno").ShouldBeTrue();
        // Add any other extension methods that make sense
    }
}
