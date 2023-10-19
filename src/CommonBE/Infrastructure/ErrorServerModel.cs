namespace CommonBE.Infrastructure;

public class ErrorServerModel
{
    //public string RequestId { get; set; }
    public string ApiRoute { get; set; }
    public int ApiStatus { get; set; }
    public string ApiErrorId { get; set; }
    public string ApiTitle { get; set; }
    public string ApiDetail { get; set; }
}
