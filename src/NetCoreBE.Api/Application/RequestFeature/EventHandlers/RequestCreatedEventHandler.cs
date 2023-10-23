// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;

namespace NetCoreBE.Api.Application.RequestFeature.EventHandlers;

public class RequestCreatedEventHandler : INotificationHandler<CreatedEvent<Request>>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RequestCreatedEventHandler> _logger;
    private readonly Stopwatch _timer;

    public RequestCreatedEventHandler(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<RequestCreatedEventHandler> logger
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _timer = new Stopwatch();
    }

    public async Task Handle(CreatedEvent<Request> notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Domain Event: {DomainEvent}, started", notification.GetType().FullName);
            _timer.Start();
            //simulate some processsing
            await Task.Delay(20 * 1000, cancellationToken);
            var add = new RequestHistory
            {
                RequestId = notification.Entity.Id,
                Operation = "Confirmed",
                Details = $"Email sent to {notification.Entity.CreatedBy} for request {notification.Entity.Id}",
                CreatedBy = $"{nameof(RequestCreatedEventHandler)}",
            };
            //must use IServiceScopeFactory. context is already disposed.
            var _logic = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IRequestLogic>();
            var res = await _logic.AddHistory(add);
            _timer.Stop();
            _logger.LogInformation("Domain Event: {DomainEvent}, end {ElapsedMilliseconds}ms", notification.GetType().FullName,
                _timer.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, null, ex);
        }
    }
}