namespace CommonBE.CustomExceptions;

/// <summary>
/// Custome 409 duplicate record Exception
/// </summary>
public class ConflictException : ApplicationException
{
	public ConflictException(string message) : base(message)
	{

	}
}
