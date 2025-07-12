using AStar.Dev.Infrastructure.FilesDb.Data;

namespace AStar.Dev.Files.Api.Endpoints;

public class FileContextExtensionsShould
{
    [Fact]
    public void ContainTheExpectedProperties()
    {
        var sut = new FilesContext();

        var response = sut.FileDetails.WhereDirectoryNameMatches("MockDirectory", true);

        response.Expression.ShouldBeNull("It shouldn't - and most likely isn't even before the test but this is a placeholder...");
    }
}
