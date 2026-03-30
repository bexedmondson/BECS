using System.Collections;

public record Entity
{
    private static int s_nextId = 0;
    
    private World world { get; init; }
    public int id { get; init; }
    
    private BitArray componentMask { get; } = new(0);
    public int componentCount
    {
        get
        {
            int count = 0;
            foreach (bool has in componentMask)
            {
                if (has)
                    count++;
            }
            return count;
        }
    }

    public Entity(World world)
    {
        this.world = world;
        id = s_nextId;
        s_nextId++;
    }

    public bool TryAdd<T>(T component) where T : IComponent
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
        
        //TODO remove in World
        return true;
    }

    public bool TryReplace<T>(T component) where T : IComponent
    {
        int index = world.GetComponentIndex(component);
        if (index >= componentMask.Length)
            return false;
        if (!componentMask.Get(world.GetComponentIndex(component)))
            return false;
        
        
        return true;
    }

    public bool TryRemove<T>() where T : IComponent
    {
        int index = world.GetComponentIndex(typeof(T));
        if (index >= componentMask.Length)
            return false;

        if (!componentMask.Get(index))
            return false;

        componentMask.Set(index, false);
        
        //TODO remove in World
        return true;
    }

    public bool Has<T>() where T : IComponent
    {
        int index = world.GetComponentIndex(typeof(T));
        if (index >= componentMask.Length)
            return false;

        return componentMask.Get(index);
    }
}
