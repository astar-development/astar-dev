using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AStar.Dev.Files.Api.Files.Add.V1;

[TestSubject(typeof(AddFilesEndpoint))]
public class AddFilesEndpointShould
{
    [Fact]
    public async Task ContainTheAddFilesEndpoint()
    {
        var sut = new AddFilesEndpoint();

        var response = await sut.AddFiles(null!) as Ok<ReadOnlyCollection<AddFilesResponse>>;

        response.ShouldNotBeNull();
        response.Value.ShouldBeEmpty();
    }
}
