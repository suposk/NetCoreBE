namespace NetCoreBE.Application.Tickets;

/// <summary>
/// Update Ticket Command
/// </summary>
public class UpdateTicketCommand : IRequest<ResultCom<TicketDto>>
{
    public required TicketUpdateDto Dto { get; set; }
}

public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, ResultCom<TicketDto>>
{
    private readonly IMapper _mapper;
    private readonly ITicketRepository _repository;
    private readonly IRepository<TicketHistory> _repositoryTicketHistory;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<UpdateTicketCommandHandler> _logger;

    public UpdateTicketCommandHandler(
        IMapper mapper,
        ITicketRepository repository,
        IRepository<TicketHistory> repositoryTicketHistory,
        IDateTimeService dateTimeService,
        ILogger<UpdateTicketCommandHandler> logger
        )
    {
        _mapper = mapper;
        _repository = repository;
        _repositoryTicketHistory = repositoryTicketHistory;
        _dateTimeService = dateTimeService;
        _logger = logger;
    }

    public async Task<ResultCom<TicketDto>> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {        
        _logger.LogInformation("Started");

        if (string.IsNullOrWhiteSpace(request.Dto.Id))
            return ResultCom<TicketDto>.Failure($"{nameof(request.Dto.Id)} parameter is missing");
        if (string.IsNullOrWhiteSpace(request.Dto.Note))
            return ResultCom<TicketDto>.Failure($"{nameof(request.Dto.Note)} parameter is missing");

        IDbContextTransaction? dbTransaction = null;
        try
        {
            dbTransaction = _repository.GetTransaction();
            _repositoryTicketHistory.UseTransaction(dbTransaction);

            var entity = await _repository.GetId(request.Dto.Id);
            if (entity is null)
                return ResultCom<TicketDto>.Failure($"Entity with id {request.Dto.Id} not found", HttpStatusCode.NotFound);

            //must include RowVersion for optimistic concurrency
            entity.RowVersion = request.Dto.RowVersion; 
            var upd = entity.Update(request.Dto.Status, request.Dto.Note, _dateTimeService.UtcNow);
            if (upd.IsFailure)
                return ResultCom<TicketDto>.Failure(upd.ErrorMessage, HttpStatusCode.BadRequest);

            var resHistory = await _repositoryTicketHistory.AddAsync(entity.TicketHistoryList.Last());

            //todo Fix invlidate cache
            entity.AddDomainEvent(new UpdatedEvent<Ticket>(entity)); //raise event to invaliated cache
            var res = await _repository.UpdateAsync(entity);
            if (res is null)
                return ResultCom<TicketDto>.Failure($"Entity with id {request.Dto.Id} failed UpdateAsync", HttpStatusCode.InternalServerError);

            dbTransaction.Commit();

            var dto = _mapper.Map<TicketDto>(res);
            _logger.LogInformation("Completed");
            return ResultCom<TicketDto>.Success(dto);
        }
        catch (Exception ex)
        {
            dbTransaction?.Rollback();
            _logger?.LogError(ex, "Error");
            return ResultCom<TicketDto>.Failure($"{ex.Message}", HttpStatusCode.InternalServerError);
        }
        finally
        {
            dbTransaction?.Dispose();
        }
    }
}