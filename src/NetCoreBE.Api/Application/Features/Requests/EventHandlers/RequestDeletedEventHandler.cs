// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace NetCoreBE.Api.Application.Features.Requests.EventHandlers;

public class RequestDeletedEventHandler : INotificationHandler<DeletedEvent<Request>>
{
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<RequestDeletedEventHandler> _logger;    

    public RequestDeletedEventHandler(
        ICacheProvider cacheProvider,
        ILogger<RequestDeletedEventHandler> logger
    )
    {
        _cacheProvider = cacheProvider;
        _logger = logger;        
    }

    public async Task Handle(DeletedEvent<Request> notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Domain Event: {DomainEvent}, started", notification.GetType().FullName);
            //_cacheProvider.ClearCache(RequestLogicCache.GetIdLogic, notification?.Entity?.Id);
            _cacheProvider.ClearCacheForAllKeysAndIds(RequestLogicCache.GetIdLogic);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, null, ex);
        }
    }
}