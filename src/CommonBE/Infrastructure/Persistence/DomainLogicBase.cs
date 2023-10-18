using AutoMapper;
using CommonBE.Infrastructure.Persistence;

namespace CSRO.Server.Infrastructure;

public interface IDomainLogicBase<TEntity, TDto> where TEntity : EntityBase where TDto : class
{
	IMapper Mapper { get; }

	Task<TDto> AddAsyncLogic(TDto dto);
	Task<TDto> AddOrUpdateAsyncLogic(TDto dto);
	Task<TDto> GetIdLogic(string id);
	Task<List<TDto>> GetListLogic();
	Task<bool> RemoveAsyncLogic(string id);
	Task<TDto> UpdateAsyncLogic(TDto dto);
}


/// <summary>
/// TODO not done
/// </summary>
/// <typeparam name="Entity"></typeparam>
public class DomainLogicBase<TEntity, TDto> : Repository<TEntity>, IDomainLogicBase<TEntity, TDto> where TEntity : EntityBase where TDto : DtoBase
{
	public DomainLogicBase(DbContext context, IApiIdentity apiIdentity, IDateTimeService dateTimeService, IMapper mapper) : base(context, apiIdentity, dateTimeService)
	{
		Mapper = mapper;
	}

	public IMapper Mapper { get; }

	public virtual async Task<List<TDto>> GetListLogic()
	{
		var repo = await GetList().ConfigureAwait(false);
		return repo == null ? null : Mapper.Map<List<TDto>>(repo);
	}

	public virtual async Task<TDto> GetIdLogic(string id)
	{
        if (id.IsNotNullValidIdExt())
            throw new BadRequestException($"{nameof(id)} {nameof(GetIdLogic)}");
        var repo = await GetId(id).ConfigureAwait(false);
		return repo == null ? null : Mapper.Map<TDto>(repo);
	}

	public virtual async Task<TDto> AddAsyncLogic(TDto dto)
	{
		if (dto.IsNotNullValidIdExt())
			throw new BadRequestException($"{nameof(dto)} {nameof(AddAsync)}");
		var repoObj = Mapper.Map<TEntity>(dto);

		await AddAsync(repoObj, dto.CreatedBy);
		var result = Mapper.Map<TDto>(repoObj);
		return result;
	}

	public virtual async Task<TDto> UpdateAsyncLogic(TDto dto)
	{
		if (dto.IsNotNullValidIdExt())			
			throw new BadRequestException($"{nameof(dto)} {nameof(UpdateAsyncLogic)}");

        var repoObj = await GetId(dto.Id.ToString()).ConfigureAwait(false);
		if (repoObj == null)
			throw new NotFoundException($"{nameof(UpdateAsyncLogic)}", dto.Id);

		repoObj = Mapper.Map<TEntity>(dto);
		if (await CanModify(repoObj, CanModifyFunc).ConfigureAwait(false))
		{
			await UpdateAsync(repoObj);
			var result = Mapper.Map<TDto>(repoObj);
			return result;
		}
		return null;
	}

	public virtual async Task<TDto> AddOrUpdateAsyncLogic(TDto dto)
	{
        if (dto.IsNotNullValidIdExt())
            throw new BadRequestException($"{nameof(dto)} {nameof(AddOrUpdateAsyncLogic)}");

		var repoObj = Mapper.Map<TEntity>(dto);
		if (await CanModify(repoObj, CanModifyFunc).ConfigureAwait(false))
		{
			if (repoObj.IsSavedInDb)
				await UpdateAsync(repoObj);
			else
				await AddAsync(repoObj, dto.CreatedBy);
			var result = Mapper.Map<TDto>(repoObj);
			return result;
		}
		return null;
	}

	public virtual async Task<bool> RemoveAsyncLogic(string id)
	{
		if (id.IsNotNullValidIdExt())
            throw new BadRequestException($"{nameof(id)} {nameof(RemoveAsyncLogic)}");

        var repoObj = await GetId(id).ConfigureAwait(false);
		if (repoObj == null)
			throw new NotFoundException($"{nameof(RemoveAsyncLogic)}", id);

		if (await CanModify(repoObj, CanModifyFunc).ConfigureAwait(false))
		{
			Remove(repoObj);
			if (await SaveChangesAsync())
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

    ///// <summary>
    ///// To add custom execution, validation must override this
    ///// </summary>
    ///// <param name="entity"></param>
    ///// <returns></returns>
    //protected virtual Task<bool> CanModifyFunc(TEntity entity) => Task.FromResult(true);
}