using System.Collections;

public class World
{
    private Dictionary<int, Entity> entities = new();
    public int entitiesCount => entities.Count;

    internal OrderedDictionary<Type, Dictionary<int, IComponent>> componentLookup = new();
    public int componentTypeCount => componentLookup.Count;

    public Action<Entity> OnEntityCreated;

    public Entity CreateEntity()
    {
        var entity = new Entity(this);
        entities[entity.id] = entity;
        OnEntityCreated?.Invoke(entity);
        return entity;
    }

    public int GetComponentIndex<T>() where T : IComponent
    {
        return GetComponentIndex(typeof(T));
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

    public void UnsetComponent<T>(Entity entity) where T : IComponent
    {
        if (componentLookup.TryGetValue(typeof(T), out var entityComponentMap))
        {
            entityComponentMap.Remove(entity.id);
        }
    }

    public T GetComponent<T>(Entity entity) where T : IComponent
    {
        Type t = typeof(T);
        if (!componentLookup.TryGetValue(t, out var entityComponentMap)
            || !entityComponentMap.TryGetValue(entity.id, out var component))
        {
            return default;
        }
        
        return (T)component;
    }

    public int GetComponentCount<T>() where T : IComponent
    {
        Type t = typeof(T);
        if (!componentLookup.TryGetValue(t, out var entityComponentMap))
        {
            return 0;
        }
        
        return entityComponentMap.Count;
    }
    
    public class Result
    {
        internal List<int> entityIds = new();
        internal Dictionary<Type, IList> componentLists = new();

        public int Count => entityIds.Count;
        
        private World world;
        
        public Result(World world)
        {
            entityIds = new List<int>(world.entities.Keys);
            this.world = world;
        }
        
        public Result Has<T>() where T : IComponent
        {
            Type t = typeof(T);
            var hasAny = world.componentLookup.TryGetValue(t, out var componentMap);
            
            if (!hasAny)
            {
                entityIds.Clear();
                foreach (var componentList in componentLists)
                {
                    componentList.Value.Clear();
                }
                return this;
            }

            var newComponentList = new List<T>(entityIds.Count);

            for (int i = entityIds.Count - 1; i >= 0; i--)
            {
                int entityId = entityIds[i];
                if (componentMap.TryGetValue(entityId, out var component))
                {
                     newComponentList.Add((T)component);
                }
                else
                {
                    entityIds.RemoveAt(i);
                    foreach (var kvp in componentLists)
                    {
                        kvp.Value.RemoveAt(i);
                    }
                }
            }

            componentLists[t] = newComponentList;

            return this;
        }
        
        public void Do(Action<Entity> action)
        {
            for (int i = 0; i < entityIds.Count; i++)
            {
                action.Invoke(world.entities[entityIds[i]]);
            }
        }

        public void Do<T>(Action<Entity, T> action) where T : IComponent
        {
            Type[] type = [typeof(T)];
            if (!HasTypes(type))
                return;
                
            for (int i = 0; i < entityIds.Count; i++)
            {
                action.Invoke(world.entities[entityIds[i]],
                    (T)componentLists[type[0]][i]
                );
            }
        }

        public void Do<T1, T2>(Action<Entity, T1, T2> action) where T1 : IComponent where T2 : IComponent
        {
            Type[] types = [typeof(T1), typeof(T2)];
            if (!HasTypes(types))
                return;
                
            for (int i = 0; i < entityIds.Count; i++)
            {
                action.Invoke(world.entities[entityIds[i]],
                    (T1)componentLists[types[0]][i],
                    (T2)componentLists[types[1]][i]
                );
            }
        }

        public void Do<T1, T2, T3>(Action<Entity, T1, T2, T3> action) where T1 : IComponent where T2 : IComponent where T3 : IComponent
        {
            Type[] types = [typeof(T1), typeof(T2), typeof(T3)];
            if (!HasTypes(types))
                return;
                
            for (int i = 0; i < entityIds.Count; i++)
            {
                action.Invoke(world.entities[entityIds[i]],
                    (T1)componentLists[types[0]][i],
                    (T2)componentLists[types[1]][i],
                    (T3)componentLists[types[2]][i]
                );
            }
        }

        private bool HasTypes(Type[] types)
        {
            foreach (var type in types)
            {
                if (!componentLists.ContainsKey(type))
                    return false;
            }
            return true;
        }
    }

    public Result Query()
    {
        return new Result(this);
    }
}