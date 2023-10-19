using MassTransit;

namespace NetCoreBE.Api.Domain.Entities;

public class Request : EntityBase
{
    [MaxLength(50)]
    public required string? RequestType { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    public List<RequestHistory> RequestHistoryList { get; set; } = new();

    public void AddInitialHistory()
    {
        if (RequestHistoryList?.Count == 0)
        {
            RequestHistoryList.Add(new RequestHistory
            {
                Id = NewId.Next().ToString(),
                RequestId = Id,
                Operation = "Submited",
                Details = "User Submited Request",
            });
        }        
    }
}
