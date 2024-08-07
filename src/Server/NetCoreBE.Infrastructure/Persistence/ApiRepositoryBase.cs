using CommonCleanArch.Application.Services;

namespace NetCoreBE.Infrastructure.Persistence;

/// <summary>
/// Required for generic repository registration
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class ApiRepositoryBase<TEntity> : Repository<TEntity> where TEntity : EntityBase
{
    public ApiRepositoryBase(ApiDbContext context, IApiIdentity apiIdentity, IDateTimeService dateTimeService) : base(context, apiIdentity, dateTimeService)
    {

    }
}
