using AutoMapper;
using Microsoft.Extensions.Logging;

namespace CommonCleanArch.Application;

public class GetByIdQuery<TDto> : IRequest<ResultCom<TDto>>    
    where TDto : IDtoBase
{
    public required string Id { get; set; }
}

///// <summary>
///// No caching 
///// </summary>
///// <typeparam name="TEntity"></typeparam>
///// <typeparam name="TDto"></typeparam>
//public class GetByIdQueryHandlerNoCache<TEntity, TDto> : GetByIdQueryHandler<TEntity, TDto>    
//    where TEntity : EntityBase
//    where TDto : IDtoBase
//{
//    public GetByIdQueryHandlerNoCache(IRepository<TEntity> repository, IApiIdentity apiIdentity, IMapper mapper, ILoggerFactory loggerFactory, ICacheProvider cacheProvider) : base(repository, apiIdentity, mapper, loggerFactory, cacheProvider)
//    {
//    }
//    public override TimeSpan? CacheDuration { get; } = null;
//}

/// <summary>
/// Version with defual cache
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TDto"></typeparam>
public class GetByIdQueryHandler<TEntity, TDto>(
        IRepository<TEntity> repository,
        IApiIdentity apiIdentity,
        IMapper mapper,
        ILoggerFactory loggerFactory,
        ICacheProvider cacheProvider
        ) : IRequestHandler<GetByIdQuery<TDto>, ResultCom<TDto>>
    where TEntity : EntityBase
    where TDto : IDtoBase
{
    /// <summary>
    /// Set custom key for cache
    /// </summary>
    public virtual string PrimaryCacheKey => $"{typeof(TEntity).Name}{Helpers.OtherHelper.CACHECONST}";

    /// <summary>
    /// Override this to add custom logic before execution, To avoid caching problems
    /// </summary>
    public virtual ValueTask OnStart(string Id) => new ValueTask(Task.CompletedTask);

    /// <summary>
    /// Disable cache or set duration
    /// </summary>
    public virtual TimeSpan? CacheDuration { get; } = TimeSpan.FromMinutes(5);
    /// <summary>
    /// replace this with your repository
    /// </summary>
    public virtual IRepository<TEntity> Repository { get; } = repository;

    protected readonly IApiIdentity ApiIdentity = apiIdentity;
    protected readonly IMapper Mapper = mapper;
    protected readonly ILoggerFactory LoggerFactory = loggerFactory;
    protected readonly ICacheProvider CacheProvider = cacheProvider;

    protected bool IsCacheEnabled => CacheDuration.HasValue && CacheDuration.Value.Minutes > 0;    
    ILogger? _ILogger;
    protected ILogger? Logger => _ILogger ??= LoggerFactory.CreateLogger(Name);
    

    string? _Name;
    public string? Name => _Name ??= ToString();

    public async Task<ResultCom<TDto>> Handle(GetByIdQuery<TDto> request, CancellationToken cancellationToken)
    {        
        try
        {
            TDto? dto;
            if (string.IsNullOrWhiteSpace(request.Id))            
                return ResultCom<TDto>.Failure($"{nameof(request.Id)} is null {Name}", HttpStatusCode.BadRequest);

            //TEntity repo;
            //if (IsCacheEnabled)
            //    repo = await CacheProvider.GetOrAddAsync(PrimaryCacheKey, (int)CacheDuration.Value.TotalSeconds, () => Repository.GetId(request.Id), request.Id);
            //else
            //    repo = await Repository.GetId(request.Id).ConfigureAwait(false);

            await OnStart(request.Id);

            TEntity repo;
            if (IsCacheEnabled)
                repo = await CacheProvider.GetOrAddAsync(PrimaryCacheKey, (int)CacheDuration.Value.TotalSeconds, () => GetById(request.Id), request.Id);
            else
                repo = await GetById(request.Id);

            if (repo == null)
            {
                Logger?.LogWarning($"{request.Id} not found");
                return ResultCom<TDto>.Failure($"{request.Id} not found", HttpStatusCode.NotFound);
            }

            if (await CanRead(repo) is false)
            {
                Logger?.LogWarning($"{request.Id} You don't have permisions to read");
                return ResultCom<TDto>.Failure($"{request.Id} You don't have permisions to read", HttpStatusCode.Unauthorized);
            }


            dto = Mapper.Map<TDto>(repo);
            Logger?.LogInformation($"OK {request.Id}", request.Id);
            return ResultCom<TDto>.Success(dto);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error");
            return ResultCom<TDto>.Failure($"{ex.Message}", HttpStatusCode.InternalServerError);            
        }        
    }

    public virtual async Task<TEntity> GetById(string id) => await Repository.GetId(id).ConfigureAwait(false);

    /// <summary>
    /// Some Optional validation for Read Operations.
    /// Must override CanReadFunc
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    protected async virtual Task<bool> CanRead(EntityBase entity, Func<EntityBase, Task<bool>> taskFunc = null)
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
    /// Only Admin or user who has permision can read record
    /// To add custom execution, validation must override this. 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected virtual Task<bool> CanReadFunc(EntityBase entity)
    {
#if DEBUG
        //TODO fix this
        return Task.FromResult(true);
#endif
        return Task.FromResult(true);
        //return _apiIdentity.CanReadFuncAsync(entity.CreatedBy);
    }

}
