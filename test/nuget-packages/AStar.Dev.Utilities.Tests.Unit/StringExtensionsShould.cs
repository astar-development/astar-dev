namespace AStar.Dev.Utilities;

public sealed class StringExtensionsShould
{
    private readonly string  anyJson          = "{\"AnyInt\":0,\"AnyString\":\"\"}";
    private readonly string  notNullString    = "value does not matter";
    private readonly string? nullString       = null;
    private readonly string  whitespaceString = " ";

    [Fact]
    public void ContainTheIsNullMethodWhichReturnsTheResult() =>
        nullString.IsNull().ShouldBeTrue();

    [Fact]
    public void ContainTheIsNotNullMethodWhichReturnsTheResult() =>
        notNullString.IsNotNull().ShouldBeTrue();

    [Fact]
    public void ContainTheIsNullOrWhiteSpaceMethodWhichReturnsTheResult() =>
        whitespaceString.IsNullOrWhiteSpace().ShouldBeTrue();

    [Fact]
    public void ContainTheIsNotNullOrWhiteSpaceMethodWhichReturnsTheResult() =>
        notNullString.IsNotNullOrWhiteSpace().ShouldBeTrue();

    [Fact]
    public void ContainTheFromJsonMethodWhichReturnsTheResult() =>
        anyJson.FromJson<AnyClass>().ShouldBeEquivalentTo(new AnyClass());

    [Fact]
    public void ContainTheFromJsonTakingJsonSerializerOptionsMethodWhichReturnsTheResult() =>
        anyJson.FromJson<AnyClass>(new()).ShouldBeEquivalentTo(new AnyClass());
}
