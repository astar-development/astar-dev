using JetBrains.Annotations;

namespace AStar.Dev.Functional.Extensions.Tests.Unit;

[TestSubject(typeof(Option<>))]
public class OptionShould
{
    [Fact]
    public void OverrideToStringForSome() => Option.Some("Test Text").ToString().ShouldBe("Some(Test Text)");

    [Fact]
    public void OverrideToStringForNone() => Option.None<string>().ToString().ShouldBe("None");
}
