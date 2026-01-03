namespace AStar.Dev.Functional.Extensions.Tests.Unit;

public class ConvenienceResultExtensionsShould
{
    [Fact]
    public void ReturnTheValueFromGetOrThrowOnSuccess()
    {
        // Arrange
        var ok = new Result<int, Exception>.Ok(123);

        // Act
        var value = ok.GetOrThrow();

        // Assert
        value.ShouldBe(123);
    }

    [Fact]
    public void ThrowTheCapturedExceptionFromGetOrThrowOnError()
    {
        // Arrange
        var ex = new InvalidOperationException("boom");
        var err = new Result<int, Exception>.Error(ex);

        // Act
        InvalidOperationException thrown = Should.Throw<InvalidOperationException>(() => err.GetOrThrow());

        // Assert
        thrown.ShouldBeSameAs(ex);
    }

    [Fact]
    public async Task ReturnTheValueFromGetOrThrowAsyncOnSuccess()
    {
        // Arrange
        Task<Result<string, Exception>> ResultTask()
        {
            return Task.FromResult<Result<string, Exception>>(new Result<string, Exception>.Ok("ok"));
        }

        // Act
        var value = await ResultTask().GetOrThrowAsync();

        // Assert
        value.ShouldBe("ok");
    }

    [Fact]
    public async Task ThrowTheCapturedExceptionFromGetOrThrowAsyncOnError()
    {
        // Arrange
        var ex = new Exception("bad");

        Task<Result<string, Exception>> ResultTask()
        {
            return Task.FromResult<Result<string, Exception>>(new Result<string, Exception>.Error(ex));
        }

        // Act
        Exception thrown = await Should.ThrowAsync<Exception>(async () => await ResultTask().GetOrThrowAsync());

        // Assert
        thrown.ShouldBeSameAs(ex);
    }

    [Fact]
    public void ReturnEmptyStringFromToErrorMessageOnSuccess()
    {
        // Arrange
        var ok = new Result<bool, Exception>.Ok(true);

        // Act
        var msg = ok.ToErrorMessage();

        // Assert
        msg.ShouldBe(string.Empty);
    }

    [Fact]
    public void ReturnExceptionMessageFromToErrorMessageOnError()
    {
        // Arrange
        var ex = new Exception("something went wrong");
        var err = new Result<object, Exception>.Error(ex);

        // Act
        var msg = err.ToErrorMessage();

        // Assert
        msg.ShouldBe("something went wrong");
    }
}
