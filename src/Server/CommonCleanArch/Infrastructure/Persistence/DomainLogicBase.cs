﻿using AutoMapper;

namespace CommonCleanArch.Infrastructure.Persistence;

public interface IDomainLogicBase<TEntity, TDto> where TEntity : EntityBase
{
    IMapper Mapper { get; }
    Task<TDto> AddAsyncLogic(TDto dto, bool saveChanges = true);
    /// <summary>
    /// /// May not be needed
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<TEntity> AddAsyncLogicEntity(TEntity entity, bool saveChanges = true);
    Task<TDto> AddOrUpdateAsyncLogic(TDto dto, bool saveChanges = true);
    /// <summary>
    /// May not be needed
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<TEntity> AddOrUpdateAsyncLogicEntity(TEntity entity, bool saveChanges = true);
    Task<TDto> GetIdLogic(string id);
    Task<List<TDto>> GetListLogic();
    Task<bool> RemoveAsyncLogic(string id, bool saveChanges = true);
    Task<TDto> UpdateAsyncLogic(TDto dto, bool saveChanges = true);
    /// <summary>
    /// /// May not be needed
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<TEntity> UpdateAsyncLogicEntity(TEntity entity, bool saveChanges = true);
}


/// <summary>
/// TODO not done
/// </summary>
/// <typeparam name="Entity"></typeparam>
public class DomainLogicBase<TEntity, TDto> : Repository<TEntity>, IDomainLogicBase<TEntity, TDto> where TEntity : EntityBase where TDto : IDtoBase
{
    public DomainLogicBase(DbContext context, IApiIdentity apiIdentity, IDateTimeService dateTimeService, IMapper mapper) : base(context, apiIdentity, dateTimeService)
    {
        Mapper = mapper;
    }

    public IMapper Mapper { get; }

    public virtual async Task<List<TDto>> GetListLogic()
    {
        var repo = await GetList().ConfigureAwait(false);
        return repo == null ? default(List<TDto>) : Mapper.Map<List<TDto>>(repo);
    }

    public virtual async Task<TDto> GetIdLogic(string id)
    {
        if (id.IsNotNullValidIdExt())
            throw new BadRequestException($"{nameof(id)} {nameof(GetIdLogic)}");
        var repo = await GetId(id).ConfigureAwait(false);
        //return repo == null ? default(TDto) : Mapper.Map<TDto>(repo);
        return repo == null ? throw new NotFoundException($"{nameof(GetIdLogic)} id", id) : Mapper.Map<TDto>(repo);
    }

    public virtual async Task<TDto> AddAsyncLogic(TDto dto, bool saveChanges = true)
    {
        if (dto.IsNotNullValidIdExt())
            throw new BadRequestException($"{nameof(dto)} {nameof(AddAsync)}");
        var repoObj = Mapper.Map<TEntity>(dto);

        await AddAsyncLogicEntity(repoObj, saveChanges);
        var result = Mapper.Map<TDto>(repoObj);
        return result;
    }

    public virtual async Task<TEntity> AddAsyncLogicEntity(TEntity entity, bool saveChanges = true)
    {
        if (entity == null)
            throw new BadRequestException($"{nameof(entity)} {nameof(AddAsyncLogicEntity)}");
        entity.AddDomainEvent(new CreatedEvent<TEntity>(entity));
        if (saveChanges)
            await AddAsync(entity, entity.CreatedBy);
        else
            Add(entity, entity.CreatedBy);
        return entity;
    }

    public virtual async Task<TDto> UpdateAsyncLogic(TDto dto, bool saveChanges = true)
    {
        if (dto.IsNotNullValidIdExt())
            throw new BadRequestException($"{nameof(dto)} {nameof(UpdateAsyncLogic)}");

        var repoObj = Mapper.Map<TEntity>(dto);
        repoObj = await UpdateAsyncLogicEntity(repoObj, saveChanges);
        if (repoObj != null)        
            return Mapper.Map<TDto>(repoObj);                    
        return default(TDto);
    }

    public virtual async Task<TEntity> UpdateAsyncLogicEntity(TEntity entity, bool saveChanges = true)
    {
        if (entity.IsNotNullValidIdExt())
            throw new BadRequestException($"{nameof(entity)} {nameof(UpdateAsyncLogicEntity)}");

        var repoObj = await GetId(entity.Id.ToString()).ConfigureAwait(false);
        if (repoObj == null)
            throw new NotFoundException($"{nameof(entity)} {nameof(UpdateAsyncLogicEntity)}", entity.Id);

        repoObj = Mapper.Map<TEntity>(entity);
        if (await CanModify(repoObj, CanModifyFunc).ConfigureAwait(false))
        {
            entity.AddDomainEvent(new UpdatedEvent<TEntity>(entity));
            if (saveChanges)
                await UpdateAsync(repoObj);
            else
                Update(repoObj);
            return repoObj;
        }
        return null;
    }

    public virtual async Task<TDto> AddOrUpdateAsyncLogic(TDto dto, bool saveChanges = true)
    {
        if (dto.IsNotNullValidIdExt())
            throw new BadRequestException($"{nameof(dto)} {nameof(AddOrUpdateAsyncLogic)}");

        var repoObj = Mapper.Map<TEntity>(dto);
        repoObj = await AddOrUpdateAsyncLogicEntity(repoObj, saveChanges);
        if (repoObj != null)
            return Mapper.Map<TDto>(repoObj);
        return default(TDto);
    }

    public virtual async Task<TEntity> AddOrUpdateAsyncLogicEntity(TEntity entity, bool saveChanges = true)
    {
        if (entity.IsNotNullValidIdExt())
            throw new BadRequestException($"{nameof(entity)} {nameof(AddOrUpdateAsyncLogicEntity)}");

        if (entity.IsSavedInDb)
        {
            if (await CanModify(entity, CanModifyFunc).ConfigureAwait(false))
            {
                entity.AddDomainEvent(new UpdatedEvent<TEntity>(entity));
                if (saveChanges)
                    await UpdateAsync(entity);
                else
                    Update(entity);                
            }
            else
                return null;
        }
        else
        {
            entity.AddDomainEvent(new CreatedEvent<TEntity>(entity));
            if (saveChanges)
                await AddAsync(entity, entity.CreatedBy);
            else
                Add(entity, entity.CreatedBy);
        }
        return entity;               
    }

    public virtual async Task<bool> RemoveAsyncLogic(string id, bool saveChanges = true)
    {
        if (id.IsNotNullValidIdExt())
            throw new BadRequestException($"{nameof(id)} {nameof(RemoveAsyncLogic)}");

        var repoObj = await GetId(id).ConfigureAwait(false);
        if (repoObj == null)
            throw new NotFoundException($"{nameof(RemoveAsyncLogic)}", id);

        if (await CanModify(repoObj, CanModifyFunc).ConfigureAwait(false))
        {
            Remove(repoObj);
            repoObj.AddDomainEvent(new DeletedEvent<TEntity>(repoObj));
            if (!saveChanges || await SaveChangesAsync())
                return true;
        }
        return false;
    }

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
    protected virtual Task<bool> CanModifyFunc(TEntity entity)
    {

#if DEBUG
        //TODO fix this
        return Task.FromResult(true);
#endif
        if (ApiIdentity.IsAdmin())
            return Task.FromResult(true);
        else
        {
            var res = entity.CreatedBy == ApiIdentity.GetUserName();
            if (!res)
                throw new UnauthorizedException($"{nameof(CanModifyFunc)} User {ApiIdentity.GetUserName()} not allowed to modify.");
            return Task.FromResult(true);
        }
    }
}