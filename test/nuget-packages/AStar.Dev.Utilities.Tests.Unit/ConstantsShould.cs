namespace AStar.Dev.Utilities;

public sealed class ConstantsShould
{
    [Fact]
    public void ContainTheExpectedWebDeserialisationSettingsSetting() =>
        Constants.WebDeserialisationSettings
                 .ToJson()
                 .ShouldMatchApproved();
}
