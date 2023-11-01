using MediatR;
using Newtonsoft.Json;
using Quartz;
using System.Text;
//using System.Text.Json;
//using System.Text.Json.Serialization;

namespace NetCoreBE.Api.Infrastructure.BackroundJobs;

[DisallowConcurrentExecution]
public class ProcessOutboxMessageDomaintEventsJob : IJob
{
    private readonly IPublisher _publisher;
    private readonly IOutboxMessageDomaintEventRepository _outboxMessageRepository;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<ProcessOutboxMessageDomaintEventsJob> _logger;

    public ProcessOutboxMessageDomaintEventsJob
        (
        IPublisher publisher, 
        IOutboxMessageDomaintEventRepository outboxMessageRepository,
        IDateTimeService dateTimeService,
        ILogger<ProcessOutboxMessageDomaintEventsJob> logger
        )
    {
        _publisher = publisher;
        _outboxMessageRepository = outboxMessageRepository;
        _dateTimeService = dateTimeService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await _outboxMessageRepository.GetListToProcess(1);
        if (messages.IsNullOrEmptyCollection())
            return; //nothing to process

        var _assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = _assemblies.SelectMany(a => a.GetTypes()).ToList();
        var typeCreatedEventGe = new CreatedEvent<Ticket>(new Ticket()).GetType();

        foreach (var message in messages) 
        {
            try
            {
                #region Generic dificlut to deserialize
                ////var type = _assemblies.SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.FullName == message.Type);
                //var type = typeCreatedEventGe;
                //if (type == null)
                //{
                //    _logger.LogWarning("Domain Event: {DomainEvent} type not found", message.Type);
                //    continue;
                //}
                //var stream = new MemoryStream(Encoding.UTF8.GetBytes(message.Content));
                //var domainEvent = JsonSerializer.Deserialize(stream, type, new JsonSerializerOptions
                //{
                //    ReferenceHandler = ReferenceHandler.Preserve
                //});
                #endregion

                //exception
                //var domainEvent = JsonSerializer.Deserialize<CreatedEvent<Ticket>>( message.Content,new JsonSerializerOptions
                //{
                //    ReferenceHandler = ReferenceHandler.Preserve
                //});

                ////exception
                //var domainEvent = System.Text.Json.JsonSerializer.Deserialize<TicketCreatedEvent>(message.Content);

                var domainEvent = JsonConvert.DeserializeObject<TicketCreatedEvent>(message.Content);
                if (domainEvent == null)
                {
                    _logger.LogWarning("Domain Event: {DomainEvent} deserialization failed", message.Type);
                    continue;
                }
                //await _publisher.Publish(domainEvent);
                //message.SetSuccess();
                message.Completed(_dateTimeService.UtcNow);
                //await _outboxMessageRepository.UpdateAsync(message);
                _logger.LogDebug("Domain Event: {DomainEvent} processed", message.Type);
            }
            catch (Exception ex)
            {
                //message.SetFailed();
                message.Failed(_dateTimeService.UtcNow, ex?.Message, _dateTimeService.UtcNow.AddMinutes(1));
                //await _outboxMessageRepository.UpdateAsync(message);    
                _logger.LogError(ex, $"{nameof(ProcessOutboxMessageDomaintEventsJob)} failed", ex);
            }
        }
    }
}
