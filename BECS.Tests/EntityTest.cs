using FluentAssertions;

public class EntityTest
{
    [Fact]
    public void TryAdd_SingleComponent_ReturnsTrue()
    {
        World world = new World();
        var entity = world.CreateEntity();
        
        entity.TryAdd(new TestComponent()).Should().BeTrue();
    }
    
    [Fact]
    public void TryAdd_SingleComponent_HasOneComponent()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
            
        entity.componentCount.Should().Be(1);
    }
    
    [Fact]
    public void TryAdd_EntityAlreadyHasComponent_ReturnsFalse()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
        
        entity.TryAdd(new TestComponent()).Should().BeFalse();
    }
    
    [Fact]
    public void TryRemove_EntityHasNoComponents_ReturnsFalse()
    {
        World world = new World();
        var entity = world.CreateEntity();
        
        entity.TryRemove<TestComponent>().Should().BeFalse();
    }
    
    [Fact]
    public void TryRemove_EntityAlreadyHasComponent_ReturnsFalse()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
        
        entity.TryRemove<TestComponent>().Should().BeTrue();
    }

    [Fact]
    public void TryRemove_ComponentAlreadyRemoved_ReturnsFalse()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
        entity.TryRemove<TestComponent>();
        
        entity.TryRemove<TestComponent>().Should().BeFalse();
    }

    [Fact]
    public void TryRemove_OtherComponentAdded_ReturnsFalse()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
        
        entity.TryRemove<TestOtherComponent>().Should().BeFalse();
    }
}
