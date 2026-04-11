using System.Collections;
using System.Text;
using System.Text.Json;

public class World
{
    protected Dictionary<int, Entity> entities = new();
    public int entitiesCount => entities.Count;

    protected Dictionary<Type, bool> shouldSerialiseTypeMap = new();

    protected OrderedDictionary<Type, Dictionary<int, IComponent>> componentLookup = new();
    public int componentTypeCount => componentLookup.Count;

    #region Events
    public Action<Entity> OnEntityCreated;
    public Action<Entity> OnEntityRemoved;
    
    private Dictionary<Type, List<Action<Entity>>> componentAddedEventMap = new();
    public void SubscribeOnComponentSet<T>(Action<Entity> subscriber) where T : IComponent
    {
        var t = typeof(T);
        if (!componentAddedEventMap.ContainsKey(t))
            componentAddedEventMap[t] = new();
        componentAddedEventMap[t].Add(subscriber);
    }
    public void UnsubscribeOnComponentSet<T>(Action<Entity> subscriber) where T : IComponent
    {
        var t = typeof(T);
        if (!componentAddedEventMap.ContainsKey(t))
            return;
        componentAddedEventMap[t].Remove(subscriber);
    }
    #endregion //Events

    public Entity CreateEntity()
    {
        var entity = new Entity(this);
        entities[entity.id] = entity;
        OnEntityCreated?.Invoke(entity);
        return entity;
    }

    public void RemoveEntity(Entity entity)
    {
        for (int i = 0; i < componentLookup.Count; i++)
        {
            if (entity.HasComponentIndex(i))
                componentLookup.GetAt(i).Value.Remove(entity.id);
        }

        entities.Remove(entity.id);
        
        OnEntityRemoved?.Invoke(entity);
    }
    
    #region ComponentHandling

    public int GetComponentIndex<T>() where T : IComponent
    {
        var t = typeof(T);
        int index = componentLookup.IndexOf(t);
        if (index != -1)
            return index;

        componentLookup[t] = new Dictionary<int, IComponent>();
        shouldSerialiseTypeMap[t] = T.shouldSerialise;
        return componentLookup.IndexOf(t);
    }

    public int GetComponentTypeIndex<T>(T componentT) where T : IComponent
    {
        var t = componentT.GetType();
        int index = componentLookup.IndexOf(t);
        if (index != -1)
            return index;

        componentLookup[t] = new Dictionary<int, IComponent>();
        shouldSerialiseTypeMap[t] = T.shouldSerialise;
        return componentLookup.IndexOf(t);
    }

    public void SetComponent<T>(Entity entity, T component) where T : IComponent
    {
        Type t = component.GetType();
        if (!componentLookup.TryGetValue(t, out var entityComponentMap))
        {
            entityComponentMap = componentLookup[t] = new Dictionary<int, IComponent>();
            
            shouldSerialiseTypeMap[t] = T.shouldSerialise;
        }
        entityComponentMap[entity.id] = component;

        if (componentAddedEventMap.TryGetValue(t, out var actions))
        {
            foreach (var action in actions)
            {
                action.Invoke(entity);
            }
        }
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
    
    #endregion //ComponentHandling
    
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
                     newComponentList[i] = (T)component;
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

    #region Serialisation
    public string Serialise(bool easyToRead = false)
    {
        OrderedDictionary<Type, string> typeToFullNameMap = new();
        foreach (var kvp in componentLookup)
        {
            if (!shouldSerialiseTypeMap[kvp.Key])
                continue;
            
            typeToFullNameMap[kvp.Key] = kvp.Key.AssemblyQualifiedName;
        }
        
        Dictionary<int, Dictionary<int, IComponent>> entityComponentMap = new();
        foreach (var kvp in entities)
        {
            entityComponentMap[kvp.Key] = new();
        }

        foreach (var typeComponentMapKvp in componentLookup)
        {
            if (!shouldSerialiseTypeMap[typeComponentMapKvp.Key])
                continue;
            
            var typeIndex = typeToFullNameMap.IndexOf(typeComponentMapKvp.Key);
            foreach (var entityComponentKvp in typeComponentMapKvp.Value)
            {
                entityComponentMap[entityComponentKvp.Key][typeIndex] = entityComponentKvp.Value;
            }
        }

        var worldData = new {
            types = typeToFullNameMap.Values,
            entities = entityComponentMap
        };

        return JsonSerializer.Serialize(
            worldData, 
            new JsonSerializerOptions{
                Converters = { new ComponentConverter() },
                WriteIndented = easyToRead,
                PropertyNameCaseInsensitive = true
            }
        );
    }

    public void Deserialise(string json)
    {
        this.entities.Clear();
        this.componentLookup.Clear();
        
        var worldData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        var typeNamesArray = ((JsonElement)worldData["types"]).Deserialize<string[]>();

        var typeMapping = new Type[typeNamesArray.Length];
        for (int i = 0; i < typeNamesArray.Length; i++)
        {
            typeMapping[i] = Type.GetType(typeNamesArray[i]);
        }

        var entityComponentMapping = ((JsonElement)worldData["entities"]).Deserialize<Dictionary<int, Dictionary<int, object>>>();
        foreach (var entityKvp in entityComponentMapping)
        {
            var entityId = entityKvp.Key;
            var componentMask = new BitArray(entityKvp.Value.Count);
            
            foreach (var componentKvp in entityKvp.Value)
            {
                componentMask[componentKvp.Key] = true;
                
                var componentType = typeMapping[componentKvp.Key];
                var component = ((JsonElement)componentKvp.Value).Deserialize(componentType);

                if (!componentLookup.TryGetValue(componentType, out var entityComponentMap))
                {
                    componentLookup[componentType] = new();
                }
                componentLookup[componentType][entityId] = (IComponent)component;
            }
            
            var newEntity = new Entity(this, entityId, componentMask);
            this.entities[entityId] = newEntity;
        }

        int maxId = 0;
        foreach (var id in entities.Keys)
        {
            if (maxId < id)
                maxId = id;
        }
        
        Entity.SetNextId(this, maxId + 1);
    }
    #endregion //Serialisation
}