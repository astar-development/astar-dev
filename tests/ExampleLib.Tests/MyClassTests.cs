
using FluentAssertions;
using ExampleLib;
using Xunit;

public class MyClassTests
{
    [Fact]
    public void Hello_ReturnsExpectedMessage()
    {
        MyClass.Hello().Should().Be("Hello from ExampleLib!");
    }
}
