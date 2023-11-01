namespace NetCoreBE.Api.Application.Features.OutboxMessageDomaintEvents;

public interface IOutboxMessageDomaintEventRepository
{
    Task<List<OutboxMessageDomaintEvent>> GetList();
    Task<List<OutboxMessageDomaintEvent>> GetListToProcess(int countToProcess = 50);
}