using CommonCleanArch;
using MediatR;
using NetCoreBE.Api.Application.Features.Tickets.EventHandlers;
using Newtonsoft.Json;
using Quartz;
using System.Reflection;
using System.Text;

namespace NetCoreBE.Api.Infrastructure.BackroundJobs;

[DisallowConcurrentExecution]
public class ProcessOutboxMessageDomaintEventsJob : IJob
{
    private readonly IPublisher _publisher;
    private readonly IOutboxMessageDomaintEventRepository _outboxMessageRepository;
    private readonly IDateTimeService _dateTimeService;
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<ProcessOutboxMessageDomaintEventsJob> _logger;

    public ProcessOutboxMessageDomaintEventsJob
        (
        IPublisher publisher, 
        IOutboxMessageDomaintEventRepository outboxMessageRepository,
        IDateTimeService dateTimeService,
        ICacheProvider cacheProvider,
        ILogger<ProcessOutboxMessageDomaintEventsJob> logger
        )
    {
        _publisher = publisher;
        _outboxMessageRepository = outboxMessageRepository;
        _dateTimeService = dateTimeService;
        _cacheProvider = cacheProvider;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await _outboxMessageRepository.GetListToProcess(1);
        if (messages.IsNullOrEmptyCollection())
            return; //nothing to process        

        //Read types from cache
        Dictionary<string?, Type> _assembliesTypesNotifications = new();
        _assembliesTypesNotifications = await LoadTypesFromCahce(_assembliesTypesNotifications);

        foreach (var message in messages)
        {
            try
            {
                if (_assembliesTypesNotifications.TryGetValue(message.Type, out var type) == false)
                {
                    _logger.LogWarning("Domain Event: {DomainEvent} type not found", message.Type);
                    continue;
                }
                var domainEvent = JsonConvert.DeserializeObject(message.Content, type);
                //var domainEvent = JsonConvert.DeserializeObject<TicketCreatedEvent>(message.Content); //works 
                if (domainEvent == null)
                {
                    _logger.LogWarning("Domain Event: {DomainEvent} deserialization failed", message.Type);
                    continue;
                }
                //await _publisher.Publish(domainEvent);                
                message.SetSuccess(_dateTimeService.UtcNow);
                //await _outboxMessageRepository.UpdateAsync(message);
                _logger.LogDebug("Domain Event: {DomainEvent} processed", message.Type);
            }
            catch (Exception ex)
            {                
                message.SetFailed(_dateTimeService.UtcNow, ex?.Message, _dateTimeService.UtcNow.AddMinutes(1));
                await _outboxMessageRepository.UpdateAsync(message);    
                _logger.LogError(ex, $"{nameof(ProcessOutboxMessageDomaintEventsJob)} failed", ex);
            }
        }
    }

    /// <summary>
    /// Read INotificationHandler types and store in the cache
    /// </summary>
    /// <param name="_assembliesTypesNotifications"></param>
    /// <returns></returns>
    private async Task<Dictionary<string?, Type>> LoadTypesFromCahce(Dictionary<string?, Type> _assembliesTypesNotifications)
    {
        _assembliesTypesNotifications = await _cacheProvider.GetOrAddAsync(nameof(ProcessOutboxMessageDomaintEventsJob), int.MaxValue, async () =>
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
        return _assembliesTypesNotifications;
    }
}
