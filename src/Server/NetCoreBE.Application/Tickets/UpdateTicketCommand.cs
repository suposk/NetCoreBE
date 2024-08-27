﻿namespace NetCoreBE.Application.Tickets;

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
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<UpdateTicketCommandHandler> _logger;

    public UpdateTicketCommandHandler(
        IMapper mapper,
        ITicketRepository repository,
        IDateTimeService dateTimeService,
        ILogger<UpdateTicketCommandHandler> logger
        )
    {
        _mapper = mapper;
        _repository = repository;
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

        try
        {
            var entity = await _repository.GetId(request.Dto.Id);
            if (entity is null)
                return ResultCom<TicketDto>.Failure($"Entity with id {request.Dto.Id} not found", HttpStatusCode.NotFound);

            if (entity.RowVersion != request.Dto.RowVersion)
                return ResultCom<TicketDto>.Failure($"Entity with id {request.Dto.Id} has been modified by another user", HttpStatusCode.Conflict);

            //todo domain entity update method  
            var upd = entity.Update(request.Dto.Status, request.Dto.Note, _dateTimeService.UtcNow);
            if (upd.IsFailure)
                return ResultCom<TicketDto>.Failure(upd.ErrorMessage, HttpStatusCode.BadRequest);

            var res = await _repository.UpdateAsync(entity);
            if (res is null)
                return ResultCom<TicketDto>.Failure($"Entity with id {request.Dto.Id} failed UpdateAsync", HttpStatusCode.InternalServerError);

            var dto = _mapper.Map<TicketDto>(res);
            _logger.LogInformation("Completed");
            return ResultCom<TicketDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error");
            return ResultCom<TicketDto>.Failure($"{ex.Message}", HttpStatusCode.InternalServerError);
        }
    }
}