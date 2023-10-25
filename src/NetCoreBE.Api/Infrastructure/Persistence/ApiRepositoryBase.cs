namespace CSRO.Server.Services.Base;

/// <summary>
/// Required for generic repository registration
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class ApiRepositoryBase<TModel> : Repository<TModel> where TModel : EntityBase
{
    public ApiRepositoryBase(ApiDbContext context, IApiIdentity apiIdentity, IDateTimeService dateTimeService) : base(context, apiIdentity, dateTimeService)
    {

    }
}
