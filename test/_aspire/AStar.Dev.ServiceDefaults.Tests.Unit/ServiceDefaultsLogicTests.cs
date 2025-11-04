namespace AStar.Dev.ServiceDefaults.Tests.Unit;

public class ServiceDefaultsLogicTests
{
    [Theory]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData("http://localhost:4317", true)]
    [InlineData("otlp-endpoint", true)]
    public void ShouldUseOtlpExporterReturnsExpectedResult(string endpoint, bool expected) => Extensions.ServiceDefaultsLogic.ShouldUseOtlpExporter(endpoint).ShouldBe(expected);
}
