namespace CommonCleanArch.Application.CustomExceptions;

/// <summary>
/// Custom 403 Exception
/// </summary>
public class ForbiddenException : ApplicationException
{
    public const string DEF_MESSAGE = "You Forbidden to perform this action";

    public ForbiddenException(string message) : base(message)
    {

    }
}

