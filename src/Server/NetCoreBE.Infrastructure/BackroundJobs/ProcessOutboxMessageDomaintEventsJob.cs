using CommonCleanArch.Domain;
using Newtonsoft.Json;
using Quartz;

namespace NetCoreBE.Infrastructure.BackroundJobs;

[DisallowConcurrentExecution]
public class ProcessOutboxDomaintEventsJob(
    IPublisher publisher,
    IOutboxDomaintEventRepository outboxDomaintEventRepository,
    IDateTimeService dateTimeService,
    ICacheProvider cacheProvider,
    ILogger<ProcessOutboxDomaintEventsJob> logger
    ) : IJob
{
    private readonly IPublisher _publisher = publisher;    
    private readonly IOutboxDomaintEventRepository _outboxDomaintEventRepository = outboxDomaintEventRepository;
    private readonly IDateTimeService _dateTimeService = dateTimeService;
    private readonly ICacheProvider _cacheProvider = cacheProvider;
    private readonly ILogger<ProcessOutboxDomaintEventsJob> _logger = logger;

    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await _outboxDomaintEventRepository.GetListToProcess(1);
        if (messages.IsNullOrEmptyCollection())
        {
            _logger.LogDebug("No OutboxDomaintEvents to process");
            return; //nothing to process        
        }

        //Read types from cache        
        Dictionary<string?, Type> _assembliesTypesNotifications = await LoadTypesFromCahce();

        foreach (var message in messages)
        {
            try
            {
                if (_assembliesTypesNotifications.TryGetValue(message.Type, out var type) == false)
                {
                    _logger.LogWarning("OutboxDomaintEvent: {DomainEvent} type not found", message.Type);
                    continue;
                }

                //test failed logic.
                //throw new Exception("Test failed logic");
                var domainEvent = JsonConvert.DeserializeObject(message.Content, type);
                var de = domainEvent as DomainEvent;
                if (de != null)
                {
                    de.SetToProcess(message.Id);
                    domainEvent = de;
                }
                //var domainEvent = JsonConvert.DeserializeObject<TicketCreatedEvent>(message.Content); //works 
                if (domainEvent == null)
                {
                    _logger.LogWarning("OutboxDomaintEvent: {DomainEvent} deserialization failed", message.Type);
                    continue;
                }                
                await _publisher.Publish(domainEvent);                
                
                //move to event handler
                message.SetProcessed(_dateTimeService.UtcNow);
                await _outboxDomaintEventRepository.UpdateAsync(message, nameof(ProcessOutboxDomaintEventsJob));                           

                _logger.LogDebug("OutboxDomaintEvent: {DomainEvent} Published", message.Type);
            }
            catch (Exception ex)
            {

                //message.SetFailed(_dateTimeService.UtcNow, ex?.Message, _dateTimeService.UtcNow.AddMinutes(1)); //add 1 minute to retry
                message.SetFailed(_dateTimeService.UtcNow, ex?.Message, _dateTimeService.UtcNow.AddSeconds(10));
                await _outboxDomaintEventRepository.UpdateAsync(message, nameof(ProcessOutboxDomaintEventsJob));    
                _logger.LogError(ex, $"{nameof(ProcessOutboxDomaintEventsJob)} failed", ex);
            }
        }
    }

    /// <summary>
    /// Read INotificationHandler types and store in the cache
    /// </summary>    
    /// <returns>Dictionary<string?, Type></returns>
    private async Task<Dictionary<string?, Type>> LoadTypesFromCahce()
    {
        Dictionary<string?, Type> assembliesTypesNotifications = await _cacheProvider.GetOrAddAsync(nameof(ProcessOutboxDomaintEventsJob), int.MaxValue, async () =>
        {
            //funguje
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            Dictionary<string?, Type> res = new();

            var types = assemblies.SelectMany(a => a.GetTypes()).ToList();
            foreach (var type in types)
            {
                if (type.IsClass && type.IsAbstract == false && type.IsGenericType == false && type.IsInterface == false)
                {
                    if (type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(INotificationHandler<>)))
                    {
                        var genericType = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(INotificationHandler<>));
                        if (genericType != null)
                        {
                            var genericArgument = genericType.GetGenericArguments().FirstOrDefault();
                            if (genericArgument != null)
                            {
                                var typeFullName = genericArgument.FullName;
                                if (typeFullName != null)
                                {
                                    if (res.ContainsKey(typeFullName) == false)
                                        res.Add(typeFullName, genericArgument);
                                }
                            }
                        }
                    }
                }
            }
            return res;
        });
        return assembliesTypesNotifications;
    }
}
