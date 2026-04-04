using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentAssertions;

[ExcludeFromCodeCoverage]
public class SerialisationTest
{
    public SerialisationTest()
    {
        var field = typeof(Entity).GetField("s_nextId", BindingFlags.Static | BindingFlags.NonPublic);
        if (field != null)
            field.SetValue(null, 0);
    }
    
    [Fact]
    public void Serialise_EmptyWorld_JsonEmptyDict()
    {
        World world = new World();
        var serialised = world.Serialise();

        serialised.Should().Be("{\"types\":[],\"entities\":{}}");
    }
    
    [Fact]
    public void Deserialise_EmptyWorld_JsonEmptyDict()
    {
        World world = new World();
        world.Deserialise("{\"types\":[],\"entities\":{}}");
        world.entitiesCount.Should().Be(0);
        world.componentTypeCount.Should().Be(0);
    }
    
    [Fact]
    public void Serialise_OneEntityWithoutComponent_IdMappedToEmptyJsonArray()
    {
        World world = new World();
        world.CreateEntity();

        var serialised = world.Serialise();

        serialised.Should().Be("{\"types\":[],\"entities\":{\"0\":{}}}");
    }
    
    [Fact]
    public void Deserialise_OneEntityWithoutComponent_IdMappedToEmptyJsonArray()
    {
        World world = new World();
        world.Deserialise("{\"types\":[],\"entities\":{\"0\":{}}}");
        world.entitiesCount.Should().Be(1);
        world.componentTypeCount.Should().Be(0);
    }

    [Fact]
    public void Serialise_OneEntityWithComponent_HasValues()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());

        var serialised = world.Serialise();

        serialised.Should().Be("{\"types\":[\"TestComponent, BECS.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"],\"entities\":{\"0\":{\"0\":{\"testInt\":123}}}}");
    }
    
    [Fact]
    public void Deserialise_OneEntityWithComponent_HasValues()
    {
        World world = new World();
        world.Deserialise("{\"types\":[\"TestComponent, BECS.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"],\"entities\":{\"0\":{\"0\":{\"testInt\":123}}}}");
        world.entitiesCount.Should().Be(1);
        world.componentTypeCount.Should().Be(1);
    }
    
    [Fact]
    public void Serialise_OneWithOneWithout_HasValuesForOne()
    {
        World world = new World();
        world.CreateEntity();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());

        var serialised = world.Serialise();

        serialised.Should().Be("{\"types\":[\"TestComponent, BECS.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"],\"entities\":{\"0\":{},\"1\":{\"0\":{\"testInt\":123}}}}");
    }
    
    [Fact]
    public void Deserialise_OneWithOneWithout_HasValuesForOne()
    {
        World world = new World();
        world.Deserialise("{\"types\":[\"TestComponent, BECS.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"],\"entities\":{\"0\":{},\"1\":{\"0\":{\"testInt\":123}}}}");
        world.entitiesCount.Should().Be(2);
        world.componentTypeCount.Should().Be(1);
    }
    
    [Fact]
    public void Serialise_OneEntityWithCustomClassComponent_HasValues()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestOtherComponent());

        var serialised = world.Serialise();

        serialised.Should().Be("{\"types\":[\"TestOtherComponent, BECS.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"],\"entities\":{\"0\":{\"0\":{\"field\":{\"hello\":\"hi\"}}}}}");
    }
    
    [Fact]
    public void Deserialise_OneEntityWithCustomClassComponent_HasValues()
    {
        World world = new World();
        world.Deserialise("{\"types\":[\"TestOtherComponent, BECS.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"],\"entities\":{\"0\":{\"0\":{\"field\":{\"hello\":\"hi\"}}}}}");
        world.entitiesCount.Should().Be(1);
        world.componentTypeCount.Should().Be(1);
    }
    
    [Fact]
    public void Serialise_TwoEntitiesWithComponent_HasValuesForBoth()
    {
        World world = new World();
        var entity1 = world.CreateEntity();
        entity1.TryAdd(new TestComponent());
        var entity2 = world.CreateEntity();
        entity2.TryAdd(new TestComponent(){testInt = 234});

        var serialised = world.Serialise();

        serialised.Should().Be("{\"types\":[\"TestComponent, BECS.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"],\"entities\":{\"0\":{\"0\":{\"testInt\":123}},\"1\":{\"0\":{\"testInt\":234}}}}");
    }
    
    [Fact]
    public void Deserialise_TwoEntitiesWithComponent_HasValuesForBoth()
    {
        World world = new World();
        world.Deserialise("{\"types\":[\"TestComponent, BECS.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"],\"entities\":{\"0\":{\"0\":{\"testInt\":123}},\"1\":{\"0\":{\"testInt\":234}}}}");
        world.entitiesCount.Should().Be(2);
        world.componentTypeCount.Should().Be(1);
    }
    
    [Fact]
    public void Serialise_OneEntitiesWithTwoComponents_HasValuesForBoth()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());
        entity.TryAdd(new TestOtherComponent());

        var serialised = world.Serialise();

        serialised.Should().Be("{\"types\":[\"TestComponent, BECS.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"TestOtherComponent, BECS.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"],\"entities\":{\"0\":{\"0\":{\"testInt\":123},\"1\":{\"field\":{\"hello\":\"hi\"}}}}}");
    }
    
    [Fact]
    public void Deserialise_OneEntitiesWithTwoComponents_HasValuesForBoth()
    {
        World world = new World();
        world.Deserialise("{\"types\":[\"TestComponent, BECS.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"TestOtherComponent, BECS.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"],\"entities\":{\"0\":{\"0\":{\"testInt\":123},\"1\":{\"field\":{\"hello\":\"hi\"}}}}}");
        world.entitiesCount.Should().Be(1);
        world.componentTypeCount.Should().Be(2);
    }
}
