namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class FileDimensionsWithSizeShould
{
    [Fact]
    public void ReturnTheExpectedToString() =>
        new FileDimensionsWithSize { Width = 456, Height = 123, FileLength = 1234 }.ToString().ShouldMatchApproved();
}
