using System.Collections;

public record Entity
{
    private static int nextId = 0;
    
    private World world { get; init; }
    public int id { get; init; }

    public Entity(World world)
    {
        this.world = world;
        id = nextId;
        nextId++;
    }
    
    public BitArray componentMask { get; } = new(0);

    public bool TryAdd(IComponent component)
    {
        int index = world.GetComponentIndex(component);
        if (index >= componentMask.Length)
        {
            componentMask.Length = index + 1;
            componentMask.Set(index, true);
            return true;
        }
        if (componentMask.Get(index))
            return false;
        
        componentMask.Set(index, true);
        return true;
    }

    public bool TryReplace(IComponent component)
    {
        int index = world.GetComponentIndex(component);
        if (index >= componentMask.Length)
            return false;
        if (!componentMask.Get(world.GetComponentIndex(component)))
            return false;
        
        
        return true;
    }

    public bool TryRemove(IComponent component)
    {
        return false;
    }

    public bool Has<T>() where T : IComponent
    {
        
        return false;
    }
}
