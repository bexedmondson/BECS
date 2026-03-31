# Basic Entity Component System, or BECS

A minimal-feature ECS library for C# with intuitive syntax.

## Examples

### Creating
A world:
```csharp
World world = new World();
```

An entity:
```csharp
Entity entity = world.CreateEntity();
```

A component:
```csharp
public class ExampleComponent : IComponent { }
```

### Using
Adding a component:
```csharp
entity.TryAdd(new ExampleComponent());
```

Replacing a component:
```csharp
entity.TryReplace(new ExampleComponent());
```

Removing a component:
```csharp
entity.TryRemove<ExampleComponent>();
```

Checking if an entity has a component:
```csharp
bool has = entity.Has<ExampleComponent>();
```

Querying the world for entities with a component and calling a method with each compatible entity:
```csharp
public void ExampleMethod(Entity entity, ExampleComponent component)
{
    //do something here
}

...

world.Query().Has<ExampleComponent>().Do(ExampleMethod);
```
