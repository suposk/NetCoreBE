using NetCoreBE.Api.Infrastructure.Persistence;

namespace CSRO.Server.Services.Base
{
    public class ApiBaseRepository<TModel> : Repository<TModel>, IRepository<TModel> where TModel : EntityBase
    {
        public ApiBaseRepository(ApiDbContext context, IApiIdentity apiIdentity, IDateTimeService dateTimeService) : base(context, apiIdentity, dateTimeService)
        {

        }
    }
}
