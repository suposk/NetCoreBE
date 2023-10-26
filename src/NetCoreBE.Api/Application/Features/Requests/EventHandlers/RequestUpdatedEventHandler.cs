// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace NetCoreBE.Api.Application.Features.Requests.EventHandlers;

public class RequestUpdatedEventHandler : INotificationHandler<UpdatedEvent<Request>>
{
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<RequestUpdatedEventHandler> _logger;    

    public RequestUpdatedEventHandler(
        ICacheProvider cacheProvider,
        ILogger<RequestUpdatedEventHandler> logger
    )
    {
        _cacheProvider = cacheProvider;
        _logger = logger;        
    }

    public Task Handle(UpdatedEvent<Request> notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Domain Event: {DomainEvent}, started", notification.GetType().FullName);
            _cacheProvider.ClearCache(RequestLogicCache.GetIdLogic, notification?.Entity?.Id);            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, null, ex);
        }
        return Task.CompletedTask;
    }
}