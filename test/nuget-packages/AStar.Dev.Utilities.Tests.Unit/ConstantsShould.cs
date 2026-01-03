namespace AStar.Dev.Utilities.Tests.Unit;

public sealed class ConstantsShould
{
    [Fact]
    public void ContainTheExpectedWebDeserialisationSettingsSetting()
        => Constants.WebDeserialisationSettings
            .ToJson()
            .ShouldMatchApproved();
}
