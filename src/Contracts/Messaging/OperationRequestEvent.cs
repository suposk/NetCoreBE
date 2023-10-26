namespace Contracts.Messaging;

/// <summary>
/// Service Bus message
/// </summary>
public class OperationRequestEvent
{
	public Guid Id { get; set; }
	public DateTime CreationDateTimeUtc { get; set; }

	public OperationRequestEvent()
	{
		Id = Guid.NewGuid();
		CreationDateTimeUtc = DateTime.UtcNow;
	}
	
	public string SomeProperty { get; set; }
	public string UserId { get; set; }
}
