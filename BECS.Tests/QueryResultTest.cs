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
    public void QueryResultNot_TwoEntitiesWithComponentOneWithOther_ResultLengthOne()
    {
        World world = new World();
        var entity1 = world.CreateEntity();
        entity1.TryAdd(new TestComponent());
        entity1.TryAdd(new TestOtherComponent());
        var entity2 = world.CreateEntity();
        entity2.TryAdd(new TestComponent());

        var result = world.Query().Not<TestOtherComponent>();

        result.Count.Should().Be(1);
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
    public void QueryResultHasNot_TwoEntitiesWithComponentOneWithOther_ResultLengthOne()
    {
        World world = new World();
        var entity1 = world.CreateEntity();
        entity1.TryAdd(new TestComponent());
        entity1.TryAdd(new TestOtherComponent());
        var entity2 = world.CreateEntity();
        entity2.TryAdd(new TestComponent());

        var result = world.Query().Has<TestComponent>().Not<TestOtherComponent>();

        result.Count.Should().Be(1);
    }
    
    [Fact]
    public void QueryResultHasNot_OneEntityWithComponentOneWithOther_ResultLengthOne()
    {
        World world = new World();
        var entity1 = world.CreateEntity();
        entity1.TryAdd(new TestOtherComponent());
        var entity2 = world.CreateEntity();
        entity2.TryAdd(new TestComponent());

        var result = world.Query().Has<TestComponent>().Not<TestOtherComponent>();

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
    public void QueryResultFilter_TwoItemsOneFits_FittingEntityInResult()
    {
        World world = new World();
        var entity1 = world.CreateEntity();
        entity1.TryAdd(new TestComponent());
        var entity2 = world.CreateEntity();
        entity2.TryAdd(new TestComponent(){testInt = 234});

        var result = world.Query().Has<TestComponent>().Filter<TestComponent>(TestFilter);

        result.Count.Should().Be(1);

        int testInt = 0;
        result.Do<TestComponent>((e, tc) => testInt = tc.testInt);
        testInt.Should().Be(234);
    }
    
    [Fact]
    public void QueryResultFilter_TwoItemsOneFitsWrongParam_ExceptionThrown()
    {
        World world = new World();
        var entity1 = world.CreateEntity();
        entity1.TryAdd(new TestComponent());
        var entity2 = world.CreateEntity();
        entity2.TryAdd(new TestComponent(){testInt = 234});
        
        Action act = () => world.Query().Has<TestOtherComponent>().Filter<TestComponent>(TestFilter);

        act.Should().Throw<ComponentNotIncludedInQueryException>();
    }

    private bool TestFilter(TestComponent testComponent)
    {
        return testComponent.testInt == 234;
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
    public void QueryResultDo_OneItemQueryWrongSignature_ThrowsException()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
        int count = 0;
        
        Action act = () => world.Query().Has<TestComponent>().Do((Entity e, TestOtherComponent t) => { count++; });
        
        act.Should().Throw<ComponentNotIncludedInQueryException>();
    }
}
