namespace NetCoreBE.Api.Application.Features.Tickets;

public class ConfrimCommand : IRequest<Result>
{
    public string TicketId { get; set; }
}

public class ConfrimCommandHandler : IRequestHandler<ConfrimCommand, Result>
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

    public async Task<Result> Handle(ConfrimCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request?.TicketId))                
                return Result.Failure($"Missing parameter {nameof(request.TicketId)}");

            var ticket = await _repository.GetId(request.TicketId);
            if (ticket == null)
                return Result.Failure($"{nameof(request.TicketId)} {request.TicketId} not found.");

            ticket.SetAcceptedTicket();
            await _repository.UpdateAsync(ticket, nameof(ConfrimCommandHandler));
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ConfrimCommandHandler");              
            return Result.Failure($"Exception for {nameof(request.TicketId)}={request.TicketId} {nameof(ConfrimCommandHandler)}");
        }        
    }
}