namespace NetCoreBE.Application.OldTickets;

public class ConfrimCommand : IRequest<ResultCom>
{
    public required string TicketId { get; set; }
}

public class ConfrimCommandHandler : IRequestHandler<ConfrimCommand, ResultCom>
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IOldTicketRepository _repository;
    private readonly ILogger<ConfrimCommandHandler> _logger;

    public ConfrimCommandHandler(
        IDateTimeService dateTimeService,
        IOldTicketRepository repository,
        ILogger<ConfrimCommandHandler> logger
        )
    {
        _dateTimeService = dateTimeService;
        _repository = repository;
        _logger = logger;
    }

    public async Task<ResultCom> Handle(ConfrimCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request?.TicketId))
                return ResultCom.Failure($"Missing parameter {nameof(request.TicketId)}");

            var OldTicket = await _repository.GetId(request.TicketId);
            if (OldTicket == null)
                return ResultCom.Failure($"{nameof(request.TicketId)} {request.TicketId} not found.");

            OldTicket.SetAcceptedTicket();
            await _repository.UpdateAsync(OldTicket, nameof(ConfrimCommandHandler));
            return ResultCom.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ConfrimCommandHandler");
            return ResultCom.Failure($"Exception for {nameof(request.TicketId)}={request.TicketId} {nameof(ConfrimCommandHandler)}");
        }
    }
}