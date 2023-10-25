namespace CSRO.Server.Services.Base;

/// <summary>
/// /// <summary>
/// Required for generic logc registration
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TDto"></typeparam>
public class ApiDomainLogicBase<TEntity, TDto> : DomainLogicBase<TEntity, TDto> where TEntity : EntityBase where TDto : DtoBase
{
    public ApiDomainLogicBase(ApiDbContext context, IApiIdentity apiIdentity, IDateTimeService dateTimeService, IMapper mapper) : base(context, apiIdentity, dateTimeService, mapper)
    {

    }
}
