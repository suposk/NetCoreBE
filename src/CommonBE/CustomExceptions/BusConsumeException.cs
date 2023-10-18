namespace CommonBE.CustomExceptions;

public class BusConsumeException : ApplicationException
{
	public BusConsumeException(string message) : base(message)
	{

	}
}