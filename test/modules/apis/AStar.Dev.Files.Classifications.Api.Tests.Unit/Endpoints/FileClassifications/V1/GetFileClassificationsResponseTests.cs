using AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;

namespace AStar.Dev.Files.Classifications.Api.Tests.Unit.Endpoints.FileClassifications.V1;

public class GetFileClassificationsResponseTests
{
    [Fact]
    public void Construction_Should_Set_Values_Correctly()
    {
        var id = Guid.CreateVersion7();

        var dto = new GetFileClassificationsResponse(id, "Some Name", true, false);

        dto.Id.ShouldBe(id);
        dto.Name.ShouldBe("Some Name");
        dto.IncludeInSearch.ShouldBeTrue();
        dto.Celebrity.ShouldBeFalse();
    }

    [Fact]
    public void Deconstruction_Should_Work_As_Expected()
    {
        var id = Guid.CreateVersion7();
        var dto = new GetFileClassificationsResponse(id, "Alpha", false, true);

        var (deId, deName, deInclude, deCeleb) = dto;

        deId.ShouldBe(id);
        deName.ShouldBe("Alpha");
        deInclude.ShouldBeFalse();
        deCeleb.ShouldBeTrue();
    }

    [Fact]
    public void ValueEquality_Should_Be_Based_On_Contents()
    {
        var id = Guid.CreateVersion7();

        var a = new GetFileClassificationsResponse(id, "Zeta", true, false);
        var b = new GetFileClassificationsResponse(id, "Zeta", true, false);

        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }
}
