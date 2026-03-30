using FluentAssertions;

public class WorldTest
{
    [Fact]
    public void CreateEntity_CreateOne_ContainsOneEntity()
    {
        World world = new World();
        world.CreateEntity();

        world.entitiesCount.Should().Be(1);
    }

    [Fact]
    public void Has_EntityWithComponent_ReturnsTrue()
    {
        
    }
}
