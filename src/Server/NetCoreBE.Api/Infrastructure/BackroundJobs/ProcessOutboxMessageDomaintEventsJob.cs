﻿using MediatR;
using Newtonsoft.Json;
using Quartz;
using System.Text;

namespace NetCoreBE.Api.Infrastructure.BackroundJobs;

[DisallowConcurrentExecution]
public class ProcessOutboxMessageDomaintEventsJob : IJob
{
    private readonly IPublisher _publisher;
    private readonly IMediator _mediator;
    private readonly IOutboxMessageDomaintEventRepository _outboxMessageRepository;
    private readonly IDateTimeService _dateTimeService;
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<ProcessOutboxMessageDomaintEventsJob> _logger;

    public ProcessOutboxMessageDomaintEventsJob
        (
        IPublisher publisher, 
        IMediator mediator,
        IOutboxMessageDomaintEventRepository outboxMessageRepository,
        IDateTimeService dateTimeService,
        ICacheProvider cacheProvider,
        ILogger<ProcessOutboxMessageDomaintEventsJob> logger
        )
    {
        _publisher = publisher;
        _mediator = mediator;
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
        Dictionary<string?, Type> _assembliesTypesNotifications = await LoadTypesFromCahce();

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
                var de = domainEvent as DomainEvent;
                if (de != null)
                {
                    de.SetToProcess(message.Id);
                    domainEvent = de;
                }
                //var domainEvent = JsonConvert.DeserializeObject<TicketCreatedEvent>(message.Content); //works 
                if (domainEvent == null)
                {
                    _logger.LogWarning("Domain Event: {DomainEvent} deserialization failed", message.Type);
                    continue;
                }                
                await _publisher.Publish(domainEvent);                
                //move to event handler
                //message.SetProcessed(_dateTimeService.UtcNow);
                //await _outboxMessageRepository.UpdateAsync(message, nameof(ProcessOutboxMessageDomaintEventsJob));                           

                _logger.LogDebug("Domain Event: {DomainEvent} Published", message.Type);
            }
            catch (Exception ex)
            {                
                message.SetFailed(_dateTimeService.UtcNow, ex?.Message, _dateTimeService.UtcNow.AddMinutes(1));
                await _outboxMessageRepository.UpdateAsync(message, nameof(ProcessOutboxMessageDomaintEventsJob));    
                _logger.LogError(ex, $"{nameof(ProcessOutboxMessageDomaintEventsJob)} failed", ex);
            }
        }
    }

    /// <summary>
    /// Read INotificationHandler types and store in the cache
    /// </summary>    
    /// <returns>Dictionary<string?, Type></returns>
    private async Task<Dictionary<string?, Type>> LoadTypesFromCahce()
    {
        Dictionary<string?, Type> assembliesTypesNotifications = await _cacheProvider.GetOrAddAsync(nameof(ProcessOutboxMessageDomaintEventsJob), int.MaxValue, async () =>
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
