using AStar.Dev.Functional.Extensions;
using AStar.Dev.Utilities;

namespace AStar.Dev.Database.Updater.Core.Tests.Unit;

[TestSubject(typeof(ErrorResponse))]
public class ErrorResponseShould
{
    [Fact]
    public void InstantiateWithMessagePropertyShouldMatchInput()
    {
        const string expectedMessage = "Test error message";

        var errorResponse = new ErrorResponse(expectedMessage);

        _ = errorResponse.ShouldNotBeNull();
        errorResponse.Message.ShouldBe(expectedMessage);
    }

    [Theory]
    [InlineData("")]  // Empty string case
    [InlineData(" ")] // Whitespace only case
    [InlineData("A simple error message.")]
    [InlineData("An error message with special characters: !@#$%^&*()")]
    [InlineData("An error message with a very long length. " +
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Morbi non massa et urna fermentum consequat. " +
                "Praesent laoreet eros at turpis vehicula, nec ullamcorper felis vehicula. " +
                "Donec tincidunt vel libero vel facilisis.")] // Long message case
    public void InstantiateWithVariousMessagesMessageShouldMatchInput(string message)
    {
        var errorResponse = new ErrorResponse(message);

        _ = errorResponse.ShouldNotBeNull();
        errorResponse.Message.ShouldBe(message);
    }

    [Fact]
    public void VerifyEqualityOfTwoErrorResponseInstancesWithSameMessage()
    {
        const string message = "Test error message";

        var errorResponse1 = new ErrorResponse(message);
        var errorResponse2 = new ErrorResponse(message);

        errorResponse1.ShouldBe(errorResponse2);
    }

    [Fact]
    public void VerifyInequalityOfTwoErrorResponseInstancesWithDifferentMessages()
    {
        const string message1 = "Error message one";
        const string message2 = "Error message two";

        var errorResponse1 = new ErrorResponse(message1);
        var errorResponse2 = new ErrorResponse(message2);

        errorResponse1.ShouldNotBe(errorResponse2);
    }

    [Fact]
    public void EnsureStringRepresentationReflectsMessage()
    {
        const string message = "Error message for string representation";

        var errorResponse        = new ErrorResponse(message);
        var stringRepresentation = errorResponse.ToJson().ToString();

        stringRepresentation.ShouldContain(message);
    }
}
