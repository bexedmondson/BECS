using System.Collections;

public record Entity
{
    private static int s_nextId = 0;

    public static void SetNextId(World world, int nextId) //in an attempt to only let World call this
    {
        s_nextId = nextId;
    }
    
    protected World world { get; init; }
    public int id { get; init; }

    public Action<Entity> OnEntityUpdated;
    
    protected BitArray componentMask { get; } = new(0);
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

    public Entity(World world, int id, BitArray componentMask)
    {
        this.world = world;
        this.id = id;
        this.componentMask = componentMask;
    }

    public bool TryAdd<T>(T component) where T : IComponent
    {
        if (component.GetType().IsAbstract)
            return false;
        
        if (HasComponentType(component))
            return false;
        
        int index = world.GetComponentTypeIndex(component);
        if (index >= componentMask.Length)
        {
            componentMask.Length = index + 1;
        }
        
        componentMask.Set(index, true);
        world.SetComponent(this, component);
        
        OnEntityUpdated?.Invoke(this);
        
        return true;
    }

    public bool TryReplace<T>(T component) where T : IComponent
    {
        if (component.GetType().IsAbstract)
            return false;
        
        if (!HasComponentType(component))
            return false;
        
        world.SetComponent(this, component);
        
        OnEntityUpdated?.Invoke(this);
        
        return true;
    }

    public bool TryRemove<T>() where T : IComponent
    {
        if (!Has<T>())
            return false;

        int index = world.GetComponentIndex<T>();
        
        componentMask.Set(index, false);
        world.UnsetComponent<T>(this);
        
        OnEntityUpdated?.Invoke(this);
        
        return true;
    }
    
    private bool HasComponentType<T>(T component) where T : IComponent
    {
        if (component.GetType().IsAbstract)
            return false;
        
        int index = world.GetComponentTypeIndex(component);
        return HasComponentIndex(index);
    }

    public bool Has<T>() where T : IComponent
    {
        if (typeof(T).IsAbstract)
            return false;
        
        int index = world.GetComponentIndex<T>();
        return HasComponentIndex(index);
    }

    public bool HasComponentIndex(int componentIndex)
    {
        if (componentIndex >= componentMask.Length)
            return false;

        return componentMask.Get(componentIndex);
    }

    public bool TryGet<T>(out T component) where T : IComponent
    {
        if (!Has<T>())
        {
            component = default;
            return false;
        }

        component = world.GetComponent<T>(this);
        return true;
    }
}
