using AutoMapper;
using CommonCleanArch.Application.Services;
using Microsoft.Extensions.Logging;

namespace CommonCleanArch.Application;

public class GetListQuery<TDto> : IRequest<ResultCom<List<TDto>>>    
    where TDto : IDtoBase
{
}

public class GetListQueryHandler<TEntity, TDto>(
        IRepository<TEntity> repository,
        IApiIdentity apiIdentity,
        IMapper mapper,
        ILoggerFactory loggerFactory,
        ICacheProvider cacheProvider
        ) : IRequestHandler<GetListQuery<TDto>, ResultCom<List<TDto>>>
    where TEntity : EntityBase
    where TDto : IDtoBase
{
    /// <summary>
    /// Set custom key for cache
    /// </summary>
    public virtual string PrimaryCacheKey => $"{typeof(TEntity).Name}{Helpers.OtherHelper.CACHECONST}";
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
    protected List<TDto>? dto;
    ILogger? _ILogger;
    protected ILogger? Logger => _ILogger ??= LoggerFactory.CreateLogger(Name);    
    string? _Name;
    public string? Name => _Name ??= ToString();

    public async Task<ResultCom<List<TDto>>> Handle(GetListQuery<TDto> request, CancellationToken cancellationToken)
    {        
        try
        {
            List<TEntity> repo;
            if (IsCacheEnabled)
                repo = await CacheProvider.GetOrAddAsync(PrimaryCacheKey,(int)CacheDuration.Value.TotalSeconds, () =>  Repository.GetList());
            else
                repo = await Repository.GetList().ConfigureAwait(false);

            //if (await CanRead(repo) is false)
            //    return Result<List<TDto>>.Failure($"{request.Id} You don't have permisions to read");

            dto = Mapper.Map<List<TDto>>(repo);
            Logger?.LogInformation($"OK");
            return ResultCom<List<TDto>>.Success(dto);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error");
            return ResultCom<List<TDto>>.Failure($"{ex.Message}", HttpStatusCode.InternalServerError);            
        }        
    }

}
