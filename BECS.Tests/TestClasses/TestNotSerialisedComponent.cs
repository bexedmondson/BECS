public class TestNotSerialisedComponent : IComponent
{
    public static bool shouldSerialise => false;
    
    public bool testBool { get; set; } = true;
}
