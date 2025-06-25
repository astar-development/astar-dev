namespace AStar.Dev.Fluent.Assignments;

public sealed class FluentAssignmentsExtensionsShould
{
    private const int Value = 10;

    [Fact]
    public void AssignTheValueWhenTheCriteriaIsMatched()
    {
        var sut = new AnyClass { Id = 10.WillBeSet().IfItIs().NotNull().And().ItIsGreaterThan(5).And().ItIsLessThan(11), };

        sut.Id.ShouldBe(10);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(-10)]
    [InlineData(0)]
    public void ReturnTheSameValueFromWillBeSetWhetherNegativePositiveOrZero(int value) =>
        value.WillBeSet().ShouldBe(value);

    [Theory]
    [InlineData(10)]
    [InlineData(-10)]
    [InlineData(0)]
    public void ReturnTheSameValueFromIfItIsWhetherNegativePositiveOrZero(int value) =>
        value.IfItIs().ShouldBe(value);

    [Fact]
    public void ThrowExceptionWhenNotNullIsCalledOnNullValue()
    {
        int? nullValue = null;

        Action comparison = () => nullValue.NotNull();

        _ = comparison.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(10)]
    [InlineData(-10)]
    [InlineData(0)]
    public void ReturnTheSameValueFromTheAndExtensionWhetherNegativePositiveOrZero(int value) =>
        value.And().ShouldBe(value);

    [Theory]
    [InlineData(10)]
    [InlineData(-10)]
    [InlineData(0)]
    public void ReturnTheSameValueFromItIsGreaterThanWhenGreaterThanTheSpecifiedValue(int value) =>
        value.ItIsGreaterThan(-11).ShouldBe(value);

    [Fact]
    public void ThrowExceptionWhenItIsGreaterThanIsCalledWithValueLessThanSpecifiedMinimum()
    {
        Action comparison = () => Value.ItIsGreaterThan(Value + 1);

        comparison.ShouldThrow<ArgumentException>()
                  .Message.ShouldBe("Value 10 is not greater than 11 (Parameter 'minimum')");
    }

    [Fact]
    public void ThrowExceptionWhenItIsGreaterThanIsCalledWithValueEqualToSpecifiedMinimum()
    {
        Action comparison = () => Value.ItIsGreaterThan(Value);

        comparison.ShouldThrow<ArgumentException>()
                  .Message.ShouldBe("Value 10 is not greater than 10 (Parameter 'minimum')");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(-10)]
    [InlineData(0)]
    public void ReturnTheSameValueFromItIsLessThanWhenLessThanTheSpecifiedValue(int value) =>
        value.ItIsLessThan(value + 1).ShouldBe(value);

    [Fact]
    public void ThrowExceptionWhenItIsLessThanIsCalledWithValueLessThanSpecifiedMinimum()
    {
        Action comparison = () => Value.ItIsLessThan(Value - 1);

        comparison.ShouldThrow<ArgumentException>()
                  .Message.ShouldBe("Value 10 is not less than 9 (Parameter 'maximum')");
    }

    [Fact]
    public void ThrowExceptionWhenItIsLessThanIsCalledWithValueEqualToSpecifiedMinimum()
    {
        Action comparison = () => Value.ItIsLessThan(Value);

        comparison.ShouldThrow<ArgumentException>()
                  .Message.ShouldBe("Value 10 is not less than 10 (Parameter 'maximum')");
    }

    [Theory]
    [InlineData(10,  10)]
    [InlineData(100, 10)]
    [InlineData(-10, -10)]
    [InlineData(-5,  -10)]
    [InlineData(0,   0)]
    public void ReturnTheSameValueFromItIsGreaterThanOrEqualToWhenGreaterThanOrEqualToTheSpecifiedValue(int value,
                                                                                                        int greaterThan) =>
        value.ItIsGreaterThanOrEqualTo(greaterThan).ShouldBe(value);

    [Fact]
    public void ThrowExceptionWhenItIsGreaterThanOrEqualToIsCalledWithValueLessThanSpecifiedMinimum()
    {
        Action comparison = () => Value.ItIsGreaterThanOrEqualTo(Value + 1);

        comparison.ShouldThrow<ArgumentException>()
                  .Message.ShouldBe("Value 10 is not greater than 11 (Parameter 'minimum')");
    }

    [Theory]
    [InlineData(10,  10)]
    [InlineData(10,  100)]
    [InlineData(-10, -10)]
    [InlineData(-10, -5)]
    [InlineData(0,   0)]
    public void ReturnTheSameValueFromItIsLessThanOrEqualToWhenLessThanTheOrEqualToSpecifiedValue(int value,
                                                                                                  int lessThan) =>
        value.ItIsLessThanOrEqualTo(lessThan).ShouldBe(value);

    [Fact]
    public void ThrowExceptionWhenItIsLessThanOrEqualToIsCalledWithValueLessThanSpecifiedMinimum()
    {
        Action comparison = () => Value.ItIsLessThanOrEqualTo(Value - 1);

        comparison.ShouldThrow<ArgumentException>()
                  .Message.ShouldBe("Value 10 is not less than 9 (Parameter 'maximum')");
    }

    private class AnyClass
    {
        public int Id { get; set; }
    }
}
