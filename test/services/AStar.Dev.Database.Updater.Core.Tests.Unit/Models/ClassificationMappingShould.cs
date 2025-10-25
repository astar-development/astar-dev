using AStar.Dev.Database.Updater.Core.Models;

namespace AStar.Dev.Database.Updater.Core.Tests.Unit.Models;

public class ClassificationMappingShould
{
    [Fact]
    public void HaveDefaultPropertyValues()
    {
        var mapping = new ClassificationMapping();

        mapping.FileNameContains.ShouldBe(string.Empty);
        mapping.DatabaseMapping.ShouldBe(string.Empty);
        mapping.Celebrity.ShouldBeFalse();
        mapping.Searchable.ShouldBeFalse();
    }

    [Fact]
    public void AllowSettingProperties()
    {
        var mapping = new ClassificationMapping
        {
            FileNameContains = "IMG_",
            DatabaseMapping  = "Image",
            Celebrity        = true,
            Searchable       = true
        };

        mapping.FileNameContains.ShouldBe("IMG_");
        mapping.DatabaseMapping.ShouldBe("Image");
        mapping.Celebrity.ShouldBeTrue();
        mapping.Searchable.ShouldBeTrue();
    }
}
