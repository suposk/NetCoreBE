namespace CSRO.Server.Services.Base;

public class ApiRepositoryBase<TModel> : Repository<TModel> where TModel : EntityBase
{
    public ApiRepositoryBase(ApiDbContext context, IApiIdentity apiIdentity, IDateTimeService dateTimeService) : base(context, apiIdentity, dateTimeService)
    {

    }
}

public class ApiDomainLogicBase<TEntity, TDto> : DomainLogicBase<TEntity, TDto> where TEntity : EntityBase where TDto : DtoBase
{
    public ApiDomainLogicBase(ApiDbContext context, IApiIdentity apiIdentity, IDateTimeService dateTimeService, IMapper mapper) : base(context, apiIdentity, dateTimeService, mapper)
    {

    }
}
