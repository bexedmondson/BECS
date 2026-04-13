public class ComponentNotIncludedInQueryException : Exception
{
    public ComponentNotIncludedInQueryException()
    {
    }

    public ComponentNotIncludedInQueryException(string message)
        : base(message)
    {
    }

    public ComponentNotIncludedInQueryException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
