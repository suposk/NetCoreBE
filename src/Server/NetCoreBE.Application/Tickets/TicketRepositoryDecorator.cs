using CommonCleanArch.Application.Services;

namespace NetCoreBE.Application.Tickets;

public interface ITicketRepositoryDecorator : IRepositoryDecoratorBase<Ticket, TicketDto>
{
    Task<ResultCom<TicketDto>> UpdateDto(TicketUpdateDto? dtoUpdate);
}

public class TicketRepositoryDecorator : RepositoryDecoratorBase<Ticket, TicketDto>, ITicketRepositoryDecorator
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IRepository<TicketHistory> _repositoryTicketHistory;

    public TicketRepositoryDecorator(
        IDateTimeService dateTimeService,
        IRepository<TicketHistory> repositoryTicketHistory,
        ITicketRepository repository,
        IApiIdentity apiIdentity,
        IMapper mapper,
        ILoggerFactory logger,
        ICacheProvider cacheProvider)
        : base(repository, apiIdentity, mapper, logger, cacheProvider)
    {
        _dateTimeService = dateTimeService;
        _repositoryTicketHistory = repositoryTicketHistory;
    }

    //public override string PrimaryCacheKey => TicketCache.PrimaryCacheKey;
    public override int CacheDurationMinutes => 5;

    string? _Name;
    public override string Name => _Name ??= ToString()!;        

    protected override Task<bool> CanModifyFunc(Ticket entity) => base.CanModifyFunc(entity);

    /// <summary>
    /// not needed anymore. Holds note value from request to be added to entity.
    /// </summary>
    string? _NoteToAdd;

    //public override Task<ResultCom<TicketDto>> AddAsyncDto(TicketDto dto, bool saveChanges = true)
    //{
    //    //1. Store note value from request
    //    //dto.Notes?.Clear();
    //    //_NoteToAdd = dto.Note;
    //    return base.AddAsyncDto(dto, saveChanges);
    //}

    public async override Task<ResultCom<Ticket>> AddEntityAsync(Ticket entity, bool saveChanges = true)
    {
        //2. Add note to entity
        entity?.Init(_NoteToAdd, _dateTimeService.UtcNow);
        _NoteToAdd = null;
        return await base.AddEntityAsync(entity, saveChanges);        
    }

    public override Task<ResultCom<TicketDto>> UpdateDtoAsync(TicketDto dto, bool saveChanges = true) =>
        throw new NotImplementedException($"Use  {nameof(UpdateTicketCommand)} or {nameof(UpdateDto)} with {nameof(TicketUpdateDto)} instead");

    public async Task<ResultCom<TicketDto>> UpdateDto(TicketUpdateDto? dtoUpdate)
    {
        Logger.LogInformation("Started");

        if (dtoUpdate is null)
            return ResultCom<TicketDto>.Failure($"{nameof(dtoUpdate)} parameter is null", HttpStatusCode.BadRequest);
        if (dtoUpdate.Id.IsNullOrEmptyExt())
            return ResultCom<TicketDto>.Failure($"{nameof(dtoUpdate.Id)} parameter is missing");

        IDbContextTransaction? dbTransaction = null;
        try
        {
            dbTransaction = Repository.GetTransaction();
            _repositoryTicketHistory.UseTransaction(dbTransaction);

            var entity = await Repository.GetId(dtoUpdate.Id);
            if (entity is null)
                return ResultCom<TicketDto>.Failure($"Entity with id {dtoUpdate.Id} not found", HttpStatusCode.NotFound);

            //must include RowVersion for optimistic concurrency
            entity.RowVersion = dtoUpdate.RowVersion;
            var upd = entity.Update(dtoUpdate.Status, dtoUpdate.Note, _dateTimeService.UtcNow);
            if (upd.IsFailure)
                return ResultCom<TicketDto>.Failure(upd.ErrorMessage, HttpStatusCode.BadRequest);

            var resHistory = await _repositoryTicketHistory.AddAsync(entity.TicketHistoryList.Last());            

            entity.AddDomainEvent(new UpdatedEvent<Ticket>(entity)); //raise event to invaliated cache
            var res = await Repository.UpdateAsync(entity);
            if (res is null)
                return ResultCom<TicketDto>.Failure($"Entity with id {dtoUpdate.Id} failed UpdateAsync", HttpStatusCode.InternalServerError);

            dbTransaction?.Commit();

            var dto = Mapper.Map<TicketDto>(res);
            Logger.LogInformation("Completed");
            return ResultCom<TicketDto>.Success(dto);
        }
        catch (Exception ex)
        {
            dbTransaction?.Rollback();
            Logger.LogError(ex, "Error");
            return ResultCom<TicketDto>.Failure($"{ex.Message}", HttpStatusCode.InternalServerError);
        }
        finally
        {
            dbTransaction?.Dispose();
        }
    }

    public async override Task<ResultCom> RemoveAsync(string id, bool saveChanges = true)
    {
        //must remove all history for id
        //first mark request as deleted
        var res = await base.RemoveAsync(id, saveChanges: false);
        if (res.IsFailure)
            return res;
        else
        {
            var repoObj = await Repository.GetId(id).ConfigureAwait(false);
            foreach (var history in repoObj.TicketHistoryList)
            {
                _repositoryTicketHistory.Remove(history);
                //may be not needed here, just example
                history.AddDomainEvent(new DeletedEvent<TicketHistory>(history));
            }
            //remove reqquest & all history in one transaction
            if (saveChanges && await Repository.SaveChangesAsync())
                return ResultCom.Success();
            else
                //should not happen
                return ResultCom.Failure($"Failed to {nameof(RemoveAsync)} {id}", HttpStatusCode.InternalServerError);
        }        
    }

}
