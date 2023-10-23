namespace CSRO.Server.Services.Base;

public class ApiBaseRepository<TModel> : Repository<TModel> where TModel : EntityBase
{
    public ApiBaseRepository(ApiDbContext context, IApiIdentity apiIdentity, IDateTimeService dateTimeService) : base(context, apiIdentity, dateTimeService)
    {

    }
}


public class ApiBaseRepositoryCtx<TModel, ApiDbContext> : RepositoryCtx<TModel, ApiDbContext> where TModel : EntityBase
{
    public ApiBaseRepositoryCtx(ApiDbContext context, IApiIdentity apiIdentity, IDateTimeService dateTimeService) : base(context, apiIdentity, dateTimeService)
    {

    }
}
