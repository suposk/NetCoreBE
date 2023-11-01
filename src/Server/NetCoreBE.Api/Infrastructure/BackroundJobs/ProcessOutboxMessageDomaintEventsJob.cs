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

        var typeCreatedEventGe = new CreatedEvent<Ticket>(new Ticket()).GetType();
        //var typeDomain = new TicketCreatedEvent(new Ticket()).GetType();

        Dictionary<string?, Type> _assemblies = new();
        _assemblies = await _cacheProvider.GetOrAddAsync(nameof(ProcessOutboxMessageDomaintEventsJob), int.MaxValue, async () =>
        {
            //funguje
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();            
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
                                    if (_assemblies.ContainsKey(typeFullName) == false)
                                        _assemblies.Add(typeFullName, genericArgument);
                                }
                            }
                        }
                    }
                }
            }
            return _assemblies;
        });

        foreach (var message in messages) 
        {
            try
            {
                //var type = _assemblies.SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.FullName == message.Type);
                //var type = typeDomain;
                if (_assemblies.TryGetValue(message.Type, out var type) == false)
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
