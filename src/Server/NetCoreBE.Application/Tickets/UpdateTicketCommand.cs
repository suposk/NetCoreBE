namespace NetCoreBE.Application.Tickets;

/// <summary>
/// Only sample Command
/// </summary>
public class UpdateTicketCommand : IRequest<ResultCom<TicketDto>>
{
    public required TicketUpdateDto Dto { get; set; }
}

public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, ResultCom<TicketDto>>
{
    private readonly IMapper _mapper;
    private readonly ITicketRepository _repository;
    private readonly ILogger<UpdateTicketCommandHandler> _logger;

    public UpdateTicketCommandHandler(
        IMapper mapper,
        ITicketRepository repository,
        ILogger<UpdateTicketCommandHandler> logger
        )
    {
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }
    public async Task<ResultCom<TicketDto>> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {        
        if (string.IsNullOrWhiteSpace(request.Dto.Id))
            return ResultCom<TicketDto>.Failure($"{nameof(request.Dto.Id)} parameter is missing");
        if (string.IsNullOrWhiteSpace(request.Dto.Note))
            return ResultCom<TicketDto>.Failure($"{nameof(request.Dto.Note)} parameter is missing");

        try
        {
            var entity = await _repository.GetId(request.Dto.Id);
            if (entity is null)
                return ResultCom<TicketDto>.Failure($"Entity with id {request.Dto.Id} not found", HttpStatusCode.NotFound);

            var dto = _mapper.Map<TicketDto>(entity);
            return ResultCom<TicketDto>.Success(dto);
        }
        catch (Exception)
        {
            throw;
        }
    }
}