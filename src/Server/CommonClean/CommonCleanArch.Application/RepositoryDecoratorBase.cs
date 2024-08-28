using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace CommonCleanArch.Application;

public interface IRepositoryDecoratorBase<TEntity, TDto> where TEntity : EntityBase
{
    IMapper Mapper { get; }
    IRepository<TEntity> Repository { get; }
    DbContext DatabaseContext { get; }
    IApiIdentity ApiIdentity { get; }
    ICacheProvider CacheProvider { get; }

    Task<ResultCom<TDto>> GetIdDto(string id);
    Task<ResultCom<List<TDto>>> GetListDto();
    Task<ResultCom<TDto>> AddAsyncDto(TDto dto, bool saveChanges = true);
    Task<ResultCom<TEntity>> AddEntityAsync(TEntity entity, bool saveChanges = true);
    Task<ResultCom<TDto>> UpdateDtoAsync(TDto dto, bool saveChanges = true);
    Task<ResultCom<TEntity>> UpdateEntityAsync(TEntity entity, bool saveChanges = true);
    Task<ResultCom> RemoveAsync(string id, bool saveChanges = true);
    Task<ResultCom<TDto>> AddOrUpdateDtoAsync(TDto dto, bool saveChanges = true);
    Task<ResultCom<TEntity>> AddOrUpdateEntityAsync(TEntity entity, bool saveChanges = true);
    IDbContextTransaction GetTransaction(bool newTransaction = false);
    void UseTransaction(IDbContextTransaction transaction);
}

public abstract class RepositoryDecoratorBase<TEntity, TDto> : IRepository<TEntity>, IRepositoryDecoratorBase<TEntity, TDto> where TEntity : EntityBase where TDto : IDtoBase
{
    /// <summary>
    /// Use override string ToString() example
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Set to 0 if no caching is needed
    /// </summary>
    public abstract int CacheDurationMinutes { get; }

    /// <summary>
    /// Define the Primary Cache Key
    /// </summary>
    public virtual string PrimaryCacheKey => $"{typeof(TEntity).Name}{Helpers.OtherHelper.CACHECONST}";

    protected bool IsCacheEnabled => CacheDurationMinutes > 0;
    //protected void ResetCache()
    //{
    //    Logger?.LogInformation($"{PrimaryCacheKey} {nameof(ResetCache)}");
    //    CacheProvider.ClearCacheForAllKeysAndIds(PrimaryCacheKey);
    //}
    protected void ResetCache(string? Id)
    {
        Logger?.LogInformation($"{PrimaryCacheKey} {Id} {nameof(ResetCache)}");
        CacheProvider.ClearCacheOnlyKeyAndId(PrimaryCacheKey, Id);
    }

    public RepositoryDecoratorBase(
        IRepository<TEntity> repository,
        IApiIdentity apiIdentity,
        IMapper mapper,
        ILoggerFactory loggerFactory,
        ICacheProvider cacheProvider)
    {
        Repository = repository;
        ApiIdentity = apiIdentity;
        Mapper = mapper;
        LoggerFactory = loggerFactory;
        CacheProvider = cacheProvider;
    }

    public IMapper Mapper { get; }
    public ILoggerFactory LoggerFactory { get; }

    ILogger? _ILogger;
    public ILogger Logger => _ILogger ??= LoggerFactory.CreateLogger(Name);
    public ICacheProvider CacheProvider { get; }
    public IRepository<TEntity> Repository { get; }
    public IApiIdentity ApiIdentity { get; }
    public DbContext DatabaseContext => Repository.DatabaseContext;

    #region Get

    public virtual async Task<ResultCom<List<TDto>>> GetListDto()
    {
        Logger?.LogInformation($"{nameof(GetListDto)}");
        List<TEntity> repo;
        if (IsCacheEnabled)
            //repo = await CacheProvider.GetOrAddAsync(PrimaryCacheKey, CacheDurationMinutes * 60, () => Repository.GetList(), nameof(GetListDto));
            repo = await CacheProvider.GetOrAddAsync(PrimaryCacheKey, CacheDurationMinutes * 60, () => Repository.GetList());
        else
            repo = await Repository.GetList().ConfigureAwait(false);
        
        return repo != null ? ResultCom<List<TDto>>.Success(Mapper.Map<List<TDto>>(repo))
            : ResultCom<List<TDto>>.Failure($"{nameof(GetListDto)}", HttpStatusCode.NotFound);
    }

    public virtual async Task<ResultCom<TDto>> GetIdDto(string id)
    {
        if (id.IsNullNotValidIdExt())
            return ResultCom<TDto>.Failure($"{nameof(GetIdDto)} {nameof(id)}", HttpStatusCode.BadRequest);
        
        Logger?.LogInformation($"{nameof(GetIdDto)} {id}", id);
        TEntity repo;
        if (IsCacheEnabled)
            repo = await CacheProvider.GetOrAddAsync(PrimaryCacheKey, CacheDurationMinutes * 60, () => Repository.GetId(id), id); //same cache key and id
        else
            repo = await Repository.GetId(id).ConfigureAwait(false);
        
        return repo != null ? ResultCom<TDto>.Success(Mapper.Map<TDto>(repo)) 
            : ResultCom<TDto>.Failure($"{nameof(id)} {nameof(GetIdDto)}", HttpStatusCode.NotFound);
    }

    #endregion

    #region Add

    public virtual async Task<ResultCom<TDto>> AddAsyncDto(TDto dto, bool saveChanges = true)
    {
        if (dto.IsNullNotValidIdExt())
        {
            Logger?.LogError($"{nameof(AddAsyncDto)} {nameof(dto)} missing Id");
            return ResultCom<TDto>.Failure($"{nameof(AddAsyncDto)} {nameof(dto)} is missing Id", HttpStatusCode.BadRequest);
        }

        Logger?.LogInformation($"{nameof(AddAsyncDto)} {dto}", dto);
        var repoObj = Mapper.Map<TEntity>(dto);

        var res = await AddEntityAsync(repoObj, saveChanges);
        if (res.IsFailure)
        {
            Logger?.LogError($"{nameof(AddAsyncDto)} {res.ErrorMessage}", res.ErrorMessage);
            return ResultCom<TDto>.Failure(res.ErrorMessage, res.Error?.HttpStatusCode);
        }
        return ResultCom<TDto>.Success(Mapper.Map<TDto>(repoObj));
    }

    public virtual async Task<ResultCom<TEntity>> AddEntityAsync(TEntity entity, bool saveChanges = true)
    {
        if (entity == null)            
            return ResultCom<TEntity>.Failure($"{nameof(AddEntityAsync)} {nameof(entity)} is null", HttpStatusCode.BadRequest);

        if (await CanModify(entity, CanModifyFunc).ConfigureAwait(false))
        {
            entity.AddDomainEvent(new CreatedEvent<TEntity>(entity));
            if (saveChanges)
                await Repository.AddAsync(entity, entity.CreatedBy);
            else
                Repository.Add(entity, entity.CreatedBy);
        }
        else
            return ResultCom<TEntity>.Failure($"{nameof(AddEntityAsync)} {nameof(entity)} is {HttpStatusCode.Forbidden}", HttpStatusCode.Forbidden);

        ResetCache(entity.Id);
        return ResultCom<TEntity>.Success(entity);
    }

    #endregion

    #region Update

    public virtual async Task<ResultCom<TDto>> UpdateDtoAsync(TDto dto, bool saveChanges = true)
    {
        if (dto.IsNullNotValidIdExt())
        {
            Logger?.LogError($"{nameof(UpdateDtoAsync)} {nameof(dto)} missing Id");
            return ResultCom<TDto>.Failure($"{nameof(UpdateDtoAsync)} {nameof(dto)} is missing Id", HttpStatusCode.BadRequest);
        }
            
        var repoObj = Mapper.Map<TEntity>(dto);
        var res = await UpdateEntityAsync(repoObj, saveChanges);
        if (res.IsFailure)
        {
            Logger?.LogError($"{nameof(UpdateDtoAsync)} {res.ErrorMessage}", res.ErrorMessage);
            return ResultCom<TDto>.Failure(res.ErrorMessage, res.Error?.HttpStatusCode);
        }
        return ResultCom<TDto>.Success(Mapper.Map<TDto>(repoObj));
    }

    public virtual async Task<ResultCom<TEntity>> UpdateEntityAsync(TEntity entity, bool saveChanges = true)
    {
        if (entity is null)            
            return ResultCom<TEntity>.Failure($"{nameof(UpdateEntityAsync)} {nameof(entity)} is null", HttpStatusCode.BadRequest);

        var repoObj = await Repository.GetId(entity.Id).ConfigureAwait(false);
        if (repoObj == null)            
            return ResultCom<TEntity>.Failure($"{nameof(UpdateEntityAsync)} {nameof(entity)} {entity.Id} {HttpStatusCode.NotFound}", HttpStatusCode.NotFound);

        repoObj = Mapper.Map<TEntity>(entity);
        if (await CanModify(repoObj, CanModifyFunc).ConfigureAwait(false))
        {
            entity.AddDomainEvent(new UpdatedEvent<TEntity>(entity));
            if (saveChanges)
                await Repository.UpdateAsync(repoObj);
            else
                Repository.Update(repoObj);
        }      
        else        
            return ResultCom<TEntity>.Failure($"{nameof(UpdateEntityAsync)} {nameof(entity)} is {HttpStatusCode.Forbidden}", HttpStatusCode.Forbidden);
        
        ResetCache(entity.Id);
        return ResultCom<TEntity>.Success(entity);
    }

    #endregion

    public virtual async Task<ResultCom> RemoveAsync(string id, bool saveChanges = true)
    {
        if (id.IsNullNotValidIdExt())
        {
            Logger?.LogError($"{nameof(RemoveAsync)} missing Id");
            return ResultCom<bool>.Failure($"{nameof(RemoveAsync)} missing Id", HttpStatusCode.BadRequest);
        }
            //throw new BadRequestException(nameof(RemoveAsync));

        var repoObj = await Repository.GetId(id).ConfigureAwait(false);
        if (repoObj == null)
        {
            Logger?.LogError($"{nameof(RemoveAsync)} {id} {HttpStatusCode.NotFound}", id);
            return ResultCom.Failure($"{nameof(RemoveAsync)} {id} {HttpStatusCode.NotFound}", HttpStatusCode.NotFound);
        }    
        
        Logger?.LogInformation($"{nameof(RemoveAsync)} {id}", id);

        if (await CanModify(repoObj, CanModifyFunc).ConfigureAwait(false))
        {
            Repository.Remove(repoObj);
            repoObj.AddDomainEvent(new DeletedEvent<TEntity>(repoObj));
            if (!saveChanges || await SaveChangesAsync())
            {
                ResetCache(id);
                return ResultCom.Success();
            }
        }
        else
            return ResultCom<TEntity>.Failure($"{nameof(AddOrUpdateEntityAsync)} {id} is {HttpStatusCode.Forbidden}", HttpStatusCode.Forbidden);
        return ResultCom.Success();
    }

    #region AddOrUpdate

    public virtual async Task<ResultCom<TDto>> AddOrUpdateDtoAsync(TDto dto, bool saveChanges = true)
    {
        if (dto == null)
            throw new BadRequestException(nameof(AddOrUpdateDtoAsync));

        Logger?.LogInformation($"{nameof(AddOrUpdateDtoAsync)} {dto}", dto);
        var repoObj = Mapper.Map<TEntity>(dto);
        var res = await AddOrUpdateEntityAsync(repoObj, saveChanges);
        if (res.IsFailure)
        {
            Logger?.LogError($"{nameof(AddOrUpdateDtoAsync)} {res.ErrorMessage}", res.ErrorMessage);
            return ResultCom<TDto>.Failure(res.ErrorMessage, res.Error?.HttpStatusCode);
        }
        return ResultCom<TDto>.Success(Mapper.Map<TDto>(repoObj));
    }


    public virtual async Task<ResultCom<TEntity>> AddOrUpdateEntityAsync(TEntity entity, bool saveChanges = true)
    {
        if (entity is null)            
            return ResultCom<TEntity>.Failure($"{nameof(AddOrUpdateEntityAsync)} {nameof(entity)} is null", HttpStatusCode.BadRequest);

        if (await CanModify(entity, CanModifyFunc).ConfigureAwait(false) is false)
            return ResultCom<TEntity>.Failure($"{nameof(AddOrUpdateEntityAsync)} {nameof(entity)} is {HttpStatusCode.Forbidden}", HttpStatusCode.Forbidden);

        if (entity.IsSavedInDb)
        {
            entity.AddDomainEvent(new UpdatedEvent<TEntity>(entity));
            if (saveChanges)
                await Repository.UpdateAsync(entity);
            else
                Repository.Update(entity);
        }
        else
        {
            entity.AddDomainEvent(new CreatedEvent<TEntity>(entity));
            if (saveChanges)
                await Repository.AddAsync(entity, entity.CreatedBy);
            else
                Repository.Add(entity, entity.CreatedBy);
        }
        
        ResetCache(entity.Id); ;
        return ResultCom<TEntity>.Success(entity);
    }

    #endregion

    /// <summary>
    /// Some Optional validation for Write Operations.
    /// Must override CanModifyFunc
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    protected async virtual Task<bool> CanModify(TEntity entity, Func<TEntity, Task<bool>> taskFunc = null)
    {
        if (entity == null)
            return false;
        if (taskFunc != null)
        {
            var res = await taskFunc(entity);
            return res;
        }
        return true;
    }

    /// <summary>
    /// Only Admin or user who created can modify record
    /// To add custom execution, validation must override this.    
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected async virtual Task<bool> CanModifyFunc(TEntity entity)
    {
        //#if DEBUG
        //        //TODO fix this
        //        return Task.FromResult(true);
        //#endif
        //return ApiIdentity.CanModifyFuncAsync(entity?.CreatedBy);
        var res = await ApiIdentity.CanModifyFuncAsync(entity?.CreatedBy);
        return res ? true : throw new ForbiddenException($"{ForbiddenException.DEF_MESSAGE}. You are not owner of the resource {entity}");
    }


    #region IRepository<TEntity> methods

    public IDbContextTransaction GetTransaction(bool newTransaction = false) => Repository.GetTransaction(newTransaction);

    public void UseTransaction(IDbContextTransaction transaction) => Repository.UseTransaction(transaction);

    public Task<TEntity> GetId(string id) => Repository.GetId(id);

    public Task<List<TEntity>> GetList() => Repository.GetList();

    public void Add(TEntity entity, string UserId = null) => Repository.Add(entity, UserId);

    public void AddRange(IEnumerable<TEntity> entitys, string UserId = null) => Repository.AddRange(entitys, UserId);

    public void Remove(TEntity entity, string UserId = null) => Repository.Remove(entity, UserId);

    public void Update(TEntity entity, string UserId = null) => Repository.Update(entity, UserId);

    public Task<TEntity> AddAsync(TEntity entity, string UserId = null) => Repository.AddAsync(entity, UserId);

    public Task<TEntity> UpdateAsync(TEntity entity, string UserId = null) => Repository.UpdateAsync(entity, UserId);

    public Task<bool> SaveChangesAsync() => Repository.SaveChangesAsync();

    public Task<TEntity> GetFilter(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes) => Repository.GetFilter(expression, includes);

    public Task<TEntity> GetFilter(Expression<Func<TEntity, bool>> expression) => Repository.GetFilter(expression);

    public Task<List<TEntity>> GetListFilter(Expression<Func<TEntity, bool>> expression) => Repository.GetListFilter(expression);

    public Task<List<TEntity>> GetListFilter(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes) => Repository.GetListFilter(expression, includes);

    public void ResetAtByUser(TEntity entity) => Repository.ResetAtByUser(entity);

    //Explicitly implemented interface members
    Task<int> IRepository<TEntity>.CountAsync() => Repository.CountAsync();

    Task<bool> IRepository<TEntity>.RemoveAsync(string Id, string UserId) => Repository.RemoveAsync(Id, UserId);

    Task<List<EntitySoftDeleteBase>> IRepository<TEntity>.GetListActive(params Expression<Func<EntitySoftDeleteBase, object>>[] includes) => Repository.GetListActive(includes);

    #endregion
}