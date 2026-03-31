using System.Diagnostics.CodeAnalysis;
using FluentAssertions;

[ExcludeFromCodeCoverage]
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
    public void TryAdd_ComponentPreviouslyAddedAndRemoved_ReturnsTrue()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
        entity.TryRemove<TestComponent>();
        
        entity.TryAdd(new TestComponent()).Should().BeTrue();
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
    
    [Fact]
    public void TryReplace_EntityAlreadyHasComponent_ReturnsTrue()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
        
        entity.TryReplace(new TestComponent()).Should().BeTrue();
    }
    
    [Fact]
    public void TryReplace_SingleReplacedComponent_HasOneComponent()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
        entity.TryReplace(new TestComponent());
            
        entity.componentCount.Should().Be(1);
    }
    
    [Fact]
    public void TryReplace_SingleComponent_ReturnsFalse()
    {
        World world = new World();
        var entity = world.CreateEntity();
        
        entity.TryReplace(new TestComponent()).Should().BeFalse();
    }
    
    [Fact]
    public void TryReplace_ComponentPreviouslyAddedAndRemoved_ReturnsFalse()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
        entity.TryRemove<TestComponent>();
        
        entity.TryReplace(new TestComponent()).Should().BeFalse();
    }
    
    /*[Fact]
    public void TryReplace_SingleReplacedComponent_GetGivesNewComponent()
    {
        World world = new World();
        var entity = world.CreateEntity();
        
        var firstComponent = new TestComponent();
        entity.TryAdd(firstComponent);

        var secondComponent = new TestComponent();
        entity.TryReplace(secondComponent);

        TestComponent retrievedComponent = entity.Get<TestComponent>();

        retrievedComponent.Should().BeSameAs(secondComponent);
    }*/
    
    
    
    [Fact]
    public void TryGet_EntityHasComponentOfType_ReturnsTrueAndOutComponentOfType()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
        
        entity.TryGet<TestComponent>(out var component).Should().BeTrue();
        TestComponent castedComponent = component as TestComponent;
        (castedComponent != null).Should().BeTrue();
    }
    
    [Fact]
    public void TryGet_ComponentNotAdded_ReturnsFalse()
    {
        World world = new World();
        var entity = world.CreateEntity();
        
        entity.TryGet<TestComponent>(out _).Should().BeFalse();
    }
    
    [Fact]
    public void TryGet_ComponentPreviouslyAddedAndRemoved_ReturnsFalse()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
        entity.TryRemove<TestComponent>();
        
        entity.TryGet<TestComponent>(out _).Should().BeFalse();
    }
}
