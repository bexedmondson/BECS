using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
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

        serialised.Should().Be("{}");
    }
    
    [Fact]
    public void Serialise_OneEntityWithoutComponent_IdMappedToEmptyJsonArray()
    {
        World world = new World();
        world.CreateEntity();

        var serialised = world.Serialise();

        serialised.Should().Be("{\"0\":[]}");
    }

    [Fact]
    public void Serialise_OneEntityWithComponent_HasValues()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());

        var serialised = world.Serialise();

        serialised.Should().Be("{\"0\":[{\"testInt\":123}]}");
    }
    
    [Fact]
    public void Serialise_OneWithOneWithout_HasValuesForOne()
    {
        World world = new World();
        world.CreateEntity();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestComponent());

        var serialised = world.Serialise();

        serialised.Should().Be("{\"0\":[],\"1\":[{\"testInt\":123}]}");
    }
    
    [Fact]
    public void Serialise_OneEntityWithCustomClassComponent_HasValues()
    {
        World world = new World();
        var entity = world.CreateEntity();
        entity.TryAdd(new TestOtherComponent());

        var serialised = world.Serialise();

        serialised.Should().Be("{\"0\":[{\"field\":{\"hello\":\"hi\"}}]}");
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

        serialised.Should().Be("{\"0\":[{\"testInt\":123}],\"1\":[{\"testInt\":234}]}");
    }
}
