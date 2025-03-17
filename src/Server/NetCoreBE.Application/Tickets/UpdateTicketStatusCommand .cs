namespace NetCoreBE.Application.Tickets;

/// <summary>
/// Update Ticket Command
/// </summary>
public class UpdateTicketStatusCommand : IRequest<ResultCom<Ticket>>
{
    public required string TicketId { get; set; }
    public required StatusEnum StatusEnum { get; set; }
}

public class UpdateTicketStatusCommandHandler(
        ITicketRepository repository,
        IRepository<TicketHistory> repositoryTicketHistory,
        IDateTimeService dateTimeService,
        ILogger<UpdateTicketStatusCommandHandler> logger
        ) : IRequestHandler<UpdateTicketStatusCommand, ResultCom<Ticket>>
{    
    private readonly ITicketRepository _repository = repository;
    private readonly IRepository<TicketHistory> _repositoryTicketHistory = repositoryTicketHistory;    
    private readonly ILogger<UpdateTicketStatusCommandHandler> _logger = logger;

    public async Task<ResultCom<Ticket>> Handle(UpdateTicketStatusCommand  request, CancellationToken cancellationToken)
    {        
        _logger.LogInformation("Started");

        string ticketId = request.TicketId;
        if (string.IsNullOrWhiteSpace(ticketId))
            return ResultCom<Ticket>.Failure($"{nameof(ticketId)} parameter is missing", HttpStatusCode.BadRequest);

        IDbContextTransaction? dbTransaction = null;
        try
        {
            dbTransaction = _repository.GetTransaction();
            _repositoryTicketHistory.UseTransaction(dbTransaction);

            var entity = await _repository.GetId(ticketId);
            if (entity is null)
                return ResultCom<Ticket>.Failure($"Entity with id {ticketId} not found", HttpStatusCode.NotFound);

            if (entity.Status == request.StatusEnum.ToString())
                return ResultCom<Ticket>.Failure($"Entity with id {ticketId} is already set to {request.StatusEnum}", HttpStatusCode.Conflict);

            var upd = entity.Update(request.StatusEnum.ToString(), "Canceled by External system.", dateTimeService.UtcNow);
            if (upd.IsFailure)
                return ResultCom<Ticket>.Failure(upd.ErrorMessage, HttpStatusCode.BadRequest);

            var resHistory = await _repositoryTicketHistory.AddAsync(entity.TicketHistoryList.Last());

            entity.AddDomainEvent(new UpdatedEvent<Ticket>(entity)); //raise event to invaliated cache
            var res = await _repository.UpdateAsync(entity);
            if (res is null)
                return ResultCom<Ticket>.Failure($"Entity with id {ticketId} failed UpdateAsync", HttpStatusCode.InternalServerError);

            dbTransaction?.Commit();
                        
            _logger.LogInformation("Completed");
            return ResultCom<Ticket>.Success(entity);
        }
        catch (Exception ex)
        {
            dbTransaction?.Rollback();
            _logger?.LogError(ex, "Error");
            return ResultCom<Ticket>.Failure($"{ex.Message}", HttpStatusCode.InternalServerError);
        }
        finally
        {
            dbTransaction?.Dispose();
        }
    }
}