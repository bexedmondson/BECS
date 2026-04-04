using System.Diagnostics.CodeAnalysis;
using FluentAssertions;

[ExcludeFromCodeCoverage]
public class QueryResultTest
{
    [Fact]
    public void Query_EmptyWorld_ResultLengthZero()
    {
        World world = new World();
        var result = world.Query();

        result.Count.Should().Be(0);
    }

    [Fact]
    public void QueryResultHas_EmptyWorld_ReturnsEmptyResult()
    {
        World world = new World();

        var result = world.Query().Has<TestComponent>();

        result.Count.Should().Be(0);
    }
    
    [Fact]
    public void QueryResultHas_OneEntityWithoutComponent_ResultLengthOne()
    {
        World world = new World();
        world.CreateEntity();

        var result = world.Query().Has<TestComponent>();

        result.Count.Should().Be(0);
    }

    [Fact]
    public void QueryResultHas_OneEntityWithComponent_ResultLengthOne()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());

        var result = world.Query().Has<TestComponent>();

        result.Count.Should().Be(1);
    }
        
    [Fact]
    public void QueryResultHas_OneEntityRemovedComponent_ResultLengthZero()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
        entity.TryRemove<TestComponent>();

        var result = world.Query().Has<TestComponent>();

        result.Count.Should().Be(0);
    }
    
    [Fact]
    public void QueryResultHas_OneWithOneWithout_ResultLengthOne()
    {
        World world = new World();
        world.CreateEntity();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());

        var result = world.Query().Has<TestComponent>();

        result.Count.Should().Be(1);
    }
    
    [Fact]
    public void QueryResultHas_TwoEntitiesWithComponent_ResultLengthTwo()
    {
        World world = new World();
        var entity1 = world.CreateEntity();
        entity1.TryAdd(new TestComponent());
        var entity2 = world.CreateEntity();
        entity2.TryAdd(new TestComponent());

        var result = world.Query().Has<TestComponent>();

        result.Count.Should().Be(2);
    }
    
    [Fact]
    public void QueryResultHas_TwoEntitiesWithComponentOneWithOther_ResultLengthTwo()
    {
        World world = new World();
        var entity1 = world.CreateEntity();
        entity1.TryAdd(new TestComponent());
        entity1.TryAdd(new TestOtherComponent());
        var entity2 = world.CreateEntity();
        entity2.TryAdd(new TestComponent());

        var result = world.Query().Has<TestComponent>();

        result.Count.Should().Be(2);
    }
    
    [Fact]
    public void QueryResultHasHas_TwoEntitiesWithComponentOneWithOther_ResultLengthOne()
    {
        World world = new World();
        var entity1 = world.CreateEntity();
        entity1.TryAdd(new TestComponent());
        entity1.TryAdd(new TestOtherComponent());
        var entity2 = world.CreateEntity();
        entity2.TryAdd(new TestComponent());

        var result = world.Query().Has<TestComponent>().Has<TestOtherComponent>();

        result.Count.Should().Be(1);
    }
    
    [Fact]
    public void QueryResultHasHas_TwoEntitiesWithTwoComponents_ResultLengthTwo()
    {
        World world = new World();
        var entity1 = world.CreateEntity();
        entity1.TryAdd(new TestComponent());
        entity1.TryAdd(new TestOtherComponent());
        var entity2 = world.CreateEntity();
        entity2.TryAdd(new TestComponent());
        entity2.TryAdd(new TestOtherComponent());

        var result = world.Query().Has<TestComponent>().Has<TestOtherComponent>();

        result.Count.Should().Be(2);
    }

    [Fact]
    public void QueryResultDo_ZeroItemQuery_DoRunsZeroTimes()
    {
        World world = new World();
        int count = 0;
        
        world.Query().Has<TestComponent>().Do((Entity e) => { count++; });
        
        count.Should().Be(0);
    }

    [Fact]
    public void QueryResultDo_OneItemQuery_DoRunsOneTime()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
        int count = 0;
        
        world.Query().Has<TestComponent>().Do((Entity e, TestComponent t) => { count++; });
        
        count.Should().Be(1);
    }

    [Fact]
    public void QueryResultDo_TwoItemQuery_DoRunsTwoTimes()
    {
        World world = new World();
        var entity1 = world.CreateEntity();
        entity1.TryAdd(new TestComponent());
        var entity2 = world.CreateEntity();
        entity2.TryAdd(new TestComponent());
        int count = 0;
        
        world.Query().Has<TestComponent>().Do((Entity e, TestComponent t) => { count++; });
        
        count.Should().Be(2);
    }
    
    [Fact]
    public void QueryResultDo_OneItemQueryWrongSignature_DoRunsZeroTimes()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
        int count = 0;
        
        world.Query().Has<TestComponent>().Do((Entity e, TestOtherComponent t) => { count++; });
        
        count.Should().Be(0);
    }
}
