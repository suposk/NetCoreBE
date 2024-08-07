namespace NetCoreBE.Domain.Entities;

//public class OldTicket : EntitySoftDeleteBase
public class OldTicket : EntityBase
{
    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? RequestedFor { get; set; }

    public bool IsOnBehalf { get; set; }

    /// <summary>
    /// Active, Inactive, Pending, Completed, Cancelled
    /// </summary>
    [MaxLength(200)]
    public string? Status { get; set; }

    /// <summary>
    /// Example: Submited, Confirmed, Approved
    /// </summary>
    [MaxLength(200)]
    public string? State { get; set; }


    /// <summary>
    /// Only store in db
    /// </summary>
    public void SetNewTicket() 
    {
        Status = "Pending";
        State = "Submited";
    }

    /// <summary>
    /// Confimation email sent to user
    /// </summary>
    public void SetAcceptedTicket()
    {
        Status = "Active";
        State = "Confirmed";
    }
}
