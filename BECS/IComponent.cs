[JsonInterfaceConverter(typeof(ComponentConverter))]
public interface IComponent
{
    virtual static bool shouldSerialise { get; } = true;
}

public static class Things
{
    public static bool ShouldSerialise<T>(this T c) where T : IComponent
    {
        return T.shouldSerialise;
    }
}