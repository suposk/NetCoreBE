namespace CommonCleanArch.Application.CustomExceptions;

public class BusConsumeException : ApplicationException
{
    public BusConsumeException(string message) : base(message)
    {

    }
}