namespace CommonBE.CustomExceptions;

/// <summary>
/// Custom 404 Exception
/// </summary>
public class NotFoundException : ApplicationException
{
    public NotFoundException(string name, object key)
        : base($"{name} ({key}) is not found")
    {
    }
}
