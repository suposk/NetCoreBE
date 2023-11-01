namespace NetCoreBE.Api.Application.Features.OutboxMessageDomaintEvents;

public interface IOutboxMessageDomaintEventRepository : IRepository<OutboxMessageDomaintEvent>
{
    Task<bool> Exist(string Id, string type);
    Task<List<OutboxMessageDomaintEvent>> GetListToProcess(int countToProcess = 50);
}