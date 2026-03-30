using System;
using System.Collections.Generic;

public class World
{
    private Dictionary<int, Entity> entities = new();
    public int entitiesCount => entities.Count;

    private OrderedDictionary<Type, Dictionary<int, IComponent>> componentLookup = new();

    public Entity CreateEntity()
    {
        var entity = new Entity(this);
        entities[entity.id] = entity;
        return entity;
    }

    public int GetComponentIndex(IComponent component)
    {
        Type t = component.GetType();
        return GetComponentIndex(t);
    }
    
    public int GetComponentIndex(Type t)
    {
        int index = componentLookup.IndexOf(t);
        if (index != -1)
            return index;

        componentLookup[t] = new Dictionary<int, IComponent>();
        return componentLookup.IndexOf(t);
    }

    public void SetComponent<T>(Entity entity, T component) where T : IComponent
    {
        Type t = component.GetType();
        if (!componentLookup.TryGetValue(t, out var entityComponentMap))
        {
            entityComponentMap = componentLookup[t] = new Dictionary<int, IComponent>();
        }
        entityComponentMap[entity.id] = component;
    }

    public void UnsetComponent<T>(Entity entity, T component) where T : IComponent
    {
        Type t = component.GetType();
        if (componentLookup.TryGetValue(t, out var entityComponentMap))
        {
            entityComponentMap.Remove(entity.id);
        }
    }
    
    public class Result(World world, HashSet<int> matchingEntities) : List<(int entityId, ComponentList entityComponents)>
    {
        public Result Has<T>() where T : IComponent
        {
            var hasAny = world.componentLookup.TryGetValue(typeof(T), out var entitiesWithComponent);
            
            if (!hasAny)
            {
                this.Clear();
            }
            
            foreach (int matchingEntity in matchingEntities)
            {
                
            }

            return this;
        }
    }

    public Result Query()
    {
        HashSet<int> matchingEntities = new HashSet<int>(entities.Keys);
        return new Result(this, matchingEntities);
    }
}