namespace CommonCleanArch.Application.CustomExceptions;

/// <summary>
/// Custom 401 Exception
/// </summary>
public class UnauthorizedException : ApplicationException
{
    public const string DEF_MESSAGE = "You NOT authorized to perform this action";

    public UnauthorizedException(string message) : base(message)
    {

    }
}

