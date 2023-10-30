// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace NetCoreBE.Api.Application.Features.RequestHistorys.EventHandlers;

public class RequestHistoryCreatedEventHandler : INotificationHandler<CreatedEvent<RequestHistory>>
{
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<RequestHistoryCreatedEventHandler> _logger;

    public RequestHistoryCreatedEventHandler(
        ICacheProvider cacheProvider,
        ILogger<RequestHistoryCreatedEventHandler> logger
    )
    {
        _cacheProvider = cacheProvider;
        _logger = logger;        
    }

    public Task Handle(CreatedEvent<RequestHistory> notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Domain Event: {DomainEvent}, started", notification.GetType().FullName);
            _cacheProvider.ClearCache(RequestLogicCache.GetIdLogic, notification?.Entity?.RequestId);//clear parent cache
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, null, ex);
        }
        return Task.CompletedTask;
    }
}