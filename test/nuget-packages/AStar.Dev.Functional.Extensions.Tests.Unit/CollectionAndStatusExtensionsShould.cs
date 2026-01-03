using System.Collections.ObjectModel;

namespace AStar.Dev.Functional.Extensions.Tests.Unit;

public class CollectionAndStatusExtensionsShould
{
    [Fact]
    public async Task ReplaceTheTargetCollectionItemsOnSuccess()
    {
        // Arrange
        var target = new ObservableCollection<int> { 1, 2, 3 };

        Task<Result<IEnumerable<int>, Exception>> ResultTask()
        {
            return Task.FromResult<Result<IEnumerable<int>, Exception>>(new Result<IEnumerable<int>, Exception>.Ok([10, 20]));
        }

        // Act
        await ResultTask().ApplyToCollectionAsync(target);

        // Assert
        target.ShouldBe([10, 20]);
    }

    [Fact]
    public async Task InvokeOnErrorAndLeaveCollectionUnchangedOnFailure()
    {
        // Arrange
        var target = new ObservableCollection<string> { "a", "b" };
        var ex = new InvalidOperationException("boom");

        Task<Result<IEnumerable<string>, Exception>> ResultTask()
        {
            return Task.FromResult<Result<IEnumerable<string>, Exception>>(new Result<IEnumerable<string>, Exception>.Error(ex));
        }

        Exception? captured = null;

        // Act
        await ResultTask().ApplyToCollectionAsync(target, e => captured = e);

        // Assert
        captured.ShouldBe(ex);
        target.ShouldBe(["a", "b"]);
    }

    [Fact]
    public async Task ClearCollectionWhenSuccessValueIsNull()
    {
        // Arrange
        var target = new ObservableCollection<int> { 1, 2, 3 };

        Task<Result<IEnumerable<int>, Exception>> ResultTask()
        {
            return Task.FromResult<Result<IEnumerable<int>, Exception>>(new Result<IEnumerable<int>, Exception>.Ok(null!));
        }

        // Act
        await ResultTask().ApplyToCollectionAsync(target);

        // Assert
        target.ShouldBeEmpty();
    }

    [Fact]
    public void MapToStatusStringOnSuccess()
    {
        // Arrange
        var result = new Result<int, Exception>.Ok(42);

        // Act
        var status = result.ToStatus(i => $"Got {i}");

        // Assert
        status.ShouldBe("Got 42");
    }

    [Fact]
    public void MapToStatusStringUsingDefaultErrorMessageOnFailure()
    {
        // Arrange
        var err = new Result<int, Exception>.Error(new Exception("Failure happened"));

        // Act
        var status = err.ToStatus(_ => "Should not be used");

        // Assert
        status.ShouldBe("Failure happened");
    }

    [Fact]
    public void MapToStatusStringUsingCustomErrorFormatterOnFailure()
    {
        // Arrange
        var err = new Result<string, Exception>.Error(new InvalidOperationException("nope"));

        // Act
        var status = err.ToStatus(s => s.ToUpperInvariant(), e => $"ERR: {e.GetType().Name}:{e.Message}");

        // Assert
        status.ShouldBe("ERR: InvalidOperationException:nope");
    }
}
