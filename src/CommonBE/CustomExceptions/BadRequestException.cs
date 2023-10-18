namespace CommonBE.CustomExceptions;

/// <summary>
/// Custom 400 Exception
/// </summary>
public class BadRequestException : ApplicationException
{
    public BadRequestException(string message) : base(message)
    {

    }
}
