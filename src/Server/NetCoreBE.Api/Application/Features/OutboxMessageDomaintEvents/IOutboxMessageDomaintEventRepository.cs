namespace NetCoreBE.Api.Application.Features.OutboxMessageDomaintEvents;

public interface IOutboxMessageDomaintEventRepository : IRepository<OutboxMessageDomaintEvent>
{    
    Task<List<OutboxMessageDomaintEvent>> GetListToProcess(int countToProcess = 50);
}