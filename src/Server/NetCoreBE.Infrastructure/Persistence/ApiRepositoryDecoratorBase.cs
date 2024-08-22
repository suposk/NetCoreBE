using CommonCleanArch.Application.Services;

namespace NetCoreBE.Infrastructure.Persistence;

/// <summary>
/// /// <summary>
/// Required for generic Decorator registration
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TDto"></typeparam>
public class ApiRepositoryDecoratorBase<TEntity, TDto> : RepositoryDecoratorBase<TEntity, TDto> where TEntity : EntityBase where TDto : DtoBase
{
    public ApiRepositoryDecoratorBase(
        IRepository<TEntity> repository,
        IApiIdentity apiIdentity,
        IMapper mapper,
        ILoggerFactory loggerFactory,
        ICacheProvider cacheProvider
        )
        : base(repository, apiIdentity, mapper, loggerFactory, cacheProvider)
    {

    }

    public override int CacheDurationMinutes => 5; //posibley can can from config
    public override string PrimaryCacheKey => Name!;
    public override string Name => ToString()!;
}