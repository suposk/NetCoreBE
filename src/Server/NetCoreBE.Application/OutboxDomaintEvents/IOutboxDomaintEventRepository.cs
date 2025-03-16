using CommonCleanArch.Domain;

namespace NetCoreBE.Application.OutboxDomaintEvents;

public interface IOutboxDomaintEventRepository : IRepository<OutboxDomaintEvent>
{
    Task<bool> Exist(string entityId, string type);
    Task<List<OutboxDomaintEvent>> GetAllByEntityId(string entityId);
    Task<List<OutboxDomaintEvent>> GetAllByEntityIdType(string entityId, string type);
    Task<List<OutboxDomaintEvent>> GetListToProcess(int countToProcess = 50);
    Task<List<OutboxDomaintEvent>> GetUnProcessedList(string entityId);
    Task<List<OutboxDomaintEvent>> RemoveAllEntityId(string entityId);
    Task<bool> SaveStatus(OutboxDomaintEvent domaintEvent, bool deleteProcessed = false);
}