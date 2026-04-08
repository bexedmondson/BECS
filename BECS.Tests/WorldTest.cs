using System.Diagnostics.CodeAnalysis;
using FluentAssertions;

[ExcludeFromCodeCoverage]
public class WorldTest
{
    [Fact]
    public void CreateEntity_CreateOne_ContainsOneEntity()
    {
        World world = new World();
        world.CreateEntity();

        world.entitiesCount.Should().Be(1);
        world.GetComponentCount<TestComponent>().Should().Be(0);
    }
    
    [Fact]
    public void RemoveEntity_AfterCreateOne_ContainsZeroEntities()
    {
        World world = new World();
        var entity = world.CreateEntity();
        
        world.RemoveEntity(entity);
        
        world.entitiesCount.Should().Be(0);
    }

    [Fact]
    public void SetComponent_SetFirstTime_OneTypeRegistered()
    {
        World world = new World();
        var e = world.CreateEntity();
        
        world.SetComponent(e, new TestComponent());

        world.componentTypeCount.Should().Be(1);
    }

    [Fact]
    public void SetComponent_SetFirstTime_OneComponentOfTypeRegistered()
    {
        World world = new World();
        var e = world.CreateEntity();
        
        world.SetComponent(e, new TestComponent());

        world.GetComponentCount<TestComponent>().Should().Be(1);
    }
    
    [Fact]
    public void Query_OneEntityWorldEmptyQuery_ResultLengthOne()
    {
        World world = new World();
        world.CreateEntity();
        var result = world.Query();

        result.Count.Should().Be(1);
    }

    [Fact]
    public void GetComponent_EntityWithComponent_ReturnsSameComponent()
    {
        World world = new World();
        var entity = world.CreateEntity();
        var component = new TestComponent();
        entity.TryAdd(component);
        
        var retrievedComponent = world.GetComponent<TestComponent>(entity);

        //incomprehensibly, retrievedComponent.Should() doesn't compile
        var sameObjectEquality = retrievedComponent == component;
        sameObjectEquality.Should().BeTrue();

        var componentOfSameType = new TestComponent();
        var sameTypeEquality = retrievedComponent == componentOfSameType;
        sameTypeEquality.Should().BeFalse();
    }
    
    [Fact]
    public void GetComponent_EntityWithoutComponent_ReturnsDefault()
    {
        World world = new World();
        var entity = world.CreateEntity();
        var component = new TestComponent();
        
        var retrievedComponent = world.GetComponent<TestComponent>(entity);

        //incomprehensibly, retrievedComponent.Should() doesn't compile
        var defaultEquality = retrievedComponent == default(TestComponent);
        defaultEquality.Should().BeTrue();
    }
}
