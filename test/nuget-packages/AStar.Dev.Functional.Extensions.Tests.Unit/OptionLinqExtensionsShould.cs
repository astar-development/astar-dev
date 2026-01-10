namespace AStar.Dev.Functional.Extensions.Tests.Unit;

public class OptionLinqExtensionsShould
{
    [Fact]
    public void ProjectTheValueWithSelectWhenSome()
    {
        // Arrange
        Option<int> some = new Option<int>.Some(5);

        // Act
        Option<int> projected = some.Select(x => x * 2);

        // Assert
        projected.ShouldBeOfType<Option<int>.Some>()
            .Value.ShouldBe(10);
    }

    [Fact]
    public void PreserveNoneWithSelect()
    {
        // Arrange
        var none = Option.None<int>();

        // Act
        Option<int> projected = none.Select(x => x * 2);

        // Assert
        _ = projected.ShouldBeOfType<Option<int>.None>();
    }

    [Fact]
    public void BindAndProjectWithSelectManyWhenAllSome()
    {
        // Arrange
        Option<int> some = new Option<int>.Some(3);

        // Act
        Option<string> result = some.SelectMany(
            x => new Option<string>.Some((x * 2).ToString()),
            (x, y) => $"{x}:{y}");

        // Assert
        result.ShouldBeOfType<Option<string>.Some>()
            .Value.ShouldBe("3:6");
    }

    [Fact]
    public void SelectManyShortCircuitsWhenFirstIsNone()
    {
        // Arrange
        var none = Option.None<int>();

        // Act
        Option<string> result = none.SelectMany(
            x => new Option<string>.Some((x * 2).ToString()),
            (x, y) => $"{x}:{y}");

        // Assert
        _ = result.ShouldBeOfType<Option<string>.None>();
    }

    [Fact]
    public void SelectManyShortCircuitsWhenBindReturnsNone()
    {
        // Arrange
        Option<int> some = new Option<int>.Some(7);

        // Act
        Option<string> result = some.SelectMany(
            _ => Option.None<string>(),
            (x, y) => $"{x}:{y}");

        // Assert
        _ = result.ShouldBeOfType<Option<string>.None>();
    }

    [Fact]
    public async Task SelectAwaitProjectsAsynchronouslyWhenSome()
    {
        // Arrange
        var task = Task.FromResult<Option<int>>(new Option<int>.Some(4));

        // Act
        Option<int> projected = await task.SelectAwait(async x =>
        {
            await Task.Delay(1);
            return x * 3;
        });

        // Assert
        projected.ShouldBeOfType<Option<int>.Some>()
            .Value.ShouldBe(12);
    }

    [Fact]
    public async Task SelectAwaitPreservesNone()
    {
        var task = Task.FromResult(Option.None<int>());

        Option<int> projected = await task.SelectAwait(async x =>
        {
            await Task.Delay(1);
            return x * 3;
        });

        _ = projected.ShouldBeOfType<Option<int>.None>();
    }
}
