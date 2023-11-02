namespace NetCoreBE.Api.Application.Features.Tickets;

public class ConfrimCommand : IRequest<bool>
{
    public string TicketId { get; set; }
}

public class ConfrimCommandHandler : IRequestHandler<ConfrimCommand, bool>
{
    private readonly IDateTimeService _dateTimeService;
    private readonly ITicketRepository _repository;
    private readonly ILogger<ConfrimCommandHandler> _logger;

    public ConfrimCommandHandler(
        IDateTimeService dateTimeService,
        ITicketRepository repository,
        ILogger<ConfrimCommandHandler> logger
        )
    {
        _dateTimeService = dateTimeService;
        _repository = repository;
        _logger = logger;
    }
    public async Task<bool> Handle(ConfrimCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null || string.IsNullOrWhiteSpace(request.TicketId))
                return false;

            var ticket = await _repository.GetId(request.TicketId);
            if (ticket == null)
                return false;

            ticket.SetAcceptedTicket();
            await _repository.UpdateAsync(ticket, nameof(ConfrimCommandHandler));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ConfrimCommandHandler");  
            return false;
        }        
    }
}