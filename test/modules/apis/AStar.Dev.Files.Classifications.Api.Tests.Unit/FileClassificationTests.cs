namespace AStar.Dev.Files.Classifications.Api.Tests.Unit;

public class FileClassificationTests
{
    [Fact]
    public void Construction_Should_Set_Values_Correctly()
    {
        var id = Guid.CreateVersion7();
        var parentId = Guid.CreateVersion7();

        var model = new FileClassification(
            id,
            3,
            "Wallpapers",
            parentId,
            "Parent",
            true,
            false
        );

        model.Id.ShouldBe(id);
        model.SearchLevel.ShouldBe(3);
        model.ParentId.ShouldBe(parentId);
        model.Name.ShouldBe("Wallpapers");
        model.Celebrity.ShouldBeTrue();
        model.IncludeInSearch.ShouldBeFalse();
    }
}
