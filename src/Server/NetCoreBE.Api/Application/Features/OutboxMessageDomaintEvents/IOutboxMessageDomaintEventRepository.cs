namespace NetCoreBE.Api.Application.Features.Examples;

public interface IOutboxMessageDomaintEventRepository : IRepository<OutboxMessageDomaintEvent>
{
    Task<bool> Exist(string entityId, string type);
    Task<List<OutboxMessageDomaintEvent>> GetListToProcess(int countToProcess = 50);
    Task<List<OutboxMessageDomaintEvent>> GetUnProcessedList(string entityId);
}