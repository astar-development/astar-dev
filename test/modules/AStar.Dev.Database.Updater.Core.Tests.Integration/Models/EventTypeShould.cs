using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Database.Updater.Core.Tests.Integration.Models;

[TestSubject(typeof(EventType))]
public class EventTypeShould
{
    [Fact]
    public void InitializeAddCorrectly()
    {
        EventType.Add.Value.ShouldBe(1);
        EventType.Add.Name.ShouldBe("Add");
    }

    [Fact]
    public void InitializeUpdateCorrectly()
    {
        EventType.Update.Value.ShouldBe(2);
        EventType.Update.Name.ShouldBe("Update");
    }

    [Fact]
    public void InitializeSoftDeleteCorrectly()
    {
        EventType.SoftDelete.Value.ShouldBe(3);
        EventType.SoftDelete.Name.ShouldBe("SoftDelete");
    }

    [Fact]
    public void InitializeHardDeleteCorrectly()
    {
        EventType.HardDelete.Value.ShouldBe(4);
        EventType.HardDelete.Name.ShouldBe("HardDelete");
    }

    [Fact]
    public void OverrideToStringToReturnTheCorrectEventName()
    {
        EventType.Add.ToString().ShouldBe("Add");
        EventType.Update.ToString().ShouldBe("Update");
        EventType.HardDelete.ToString().ShouldBe("HardDelete");
        EventType.SoftDelete.ToString().ShouldBe("SoftDelete");
    }

    [Fact]
    public void BeEqualToItselfWhenComparedByReference() => EventType.Add.Equals(EventType.Add).ShouldBeTrue();

    [Fact]
    public void BeEqualToOtherInstancesWithSameValueAndName()
    {
        EventType.Add.Equals(EventType.Add).ShouldBeTrue();
        var event1 = EventType.Add;
        var event2 = EventType.Add;
        (event1 == event2).ShouldBeTrue();
    }

    [Fact]
    public void NotBeEqualToInstancesWithDifferentValues() => EventType.Add.Equals(EventType.Update).ShouldBeFalse();

    [Fact]
    public void NotBeEqualToNull() => EventType.Add.Equals(null).ShouldBeFalse();

    [Fact]
    public void NotBeEqualToADifferentObjectType()
    {
        var obj = new object();
        EventType.Add.Equals(obj).ShouldBeFalse();
    }

    [Fact]
    public void BeEqualWhenUsingEqualityOperatorWithEqualInstances()
    {
        var event1 = EventType.Add;
        var event2 = EventType.Add;
        (event1 == event2).ShouldBeTrue();
    }

    [Fact]
    public void NotBeEqualWhenUsingEqualityOperatorWithDifferentInstances() => (EventType.Add == EventType.Update).ShouldBeFalse();

    [Fact]
    public void NotBeEqualWhenLeftOperandIsNull()
    {
        EventType left = null!;
        (left == EventType.Add).ShouldBeFalse();
    }

    [Fact]
    public void NotBeEqualWhenRightOperandIsNull()
    {
        EventType right = null!;
        (EventType.Add == right).ShouldBeFalse();
    }

    [Fact]
    public void BeEqualWhenBothOperandsAreNull()
    {
        EventType left  = null!;
        EventType right = null!;
        (left == right).ShouldBeTrue();
    }

    [Fact]
    public void NotBeUnequalWhenUsingInequalityOperatorWithEqualInstances()
    {
        var event1 = EventType.Add;
        var event2 = EventType.Add;
        (event1 != event2).ShouldBeFalse();
    }

    [Fact]
    public void BeUnequalWhenUsingInequalityOperatorWithDifferentInstances() => (EventType.Add != EventType.Update).ShouldBeTrue();

    [Fact]
    public void HaveConsistentHashCodeForTheSameInstance() => EventType.Add.GetHashCode().ShouldBe(EventType.Add.GetHashCode());

    [Fact]
    public void HaveDifferentHashCodesForDifferentInstances()
    {
        EventType.Add.GetHashCode().ShouldNotBe(EventType.Update.GetHashCode());
        EventType.Add.GetHashCode().ShouldNotBe(EventType.SoftDelete.GetHashCode());
        EventType.Add.GetHashCode().ShouldNotBe(EventType.HardDelete.GetHashCode());
    }
}
