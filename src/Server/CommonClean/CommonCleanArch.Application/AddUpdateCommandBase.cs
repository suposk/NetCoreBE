//using AutoMapper;
//using CommonCleanArch.Domain.Base;
//using CommonCleanArch.Infrastructure.Persistence;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;

//namespace CommonCleanArch.Application;

//public class InputParam<EntityBase, DtoBase>
//{
//    public EntityBase? Entity { get; set; }
//    public DtoBase? Dto { get; set; }
//}

//public class OutResult<EntityBase, DtoBase>
//{
//    public EntityBase? Entity { get; set; }
//    public DtoBase? Dto { get; set; }

//    public required bool IsSuccess { get; set; }
//    public string? Message { get; set; }
//    public int? StatusCode { get; set; }
//}


//public class AddUpdateCommandBase : IRequest<OutResult<EntityBase, DtoBase>>
//{
//    public required bool IsUpdate { get; set; }
//    public required InputParam<EntityBase, DtoBase> InputParam { get; set; }
//    public bool IgnoreValidateCanModify { get; set; }
//    public bool IgnoreCheckIfEntityExist { get; set; }
//}

//public class AddUpdateCommandBaseHandler : IRequestHandler<AddUpdateCommandBase, OutResult<EntityBase, DtoBase>>
//{
//    public readonly IServiceScopeFactory _serviceScopeFactory;
//    public IApiIdentity _apiIdentity;
//    public IMapper _mapper;
//    public virtual EntityBase? Entity { get; set; }
//    public virtual DtoBase? Dto { get; set; }

//    public AddUpdateCommandBaseHandler(
//        IServiceScopeFactory serviceScopeFactory
//        )
//    {
//        _serviceScopeFactory = serviceScopeFactory;
//    }

//    public virtual async Task<OutResult<EntityBase, DtoBase>> Handle(AddUpdateCommandBase request, CancellationToken cancellationToken)
//    {
//        await Task.Yield();
//        var result = new OutResult<EntityBase, DtoBase>() { IsSuccess = false };
//        Entity = request.InputParam.Entity;
//        Dto = request.InputParam.Dto;

//        if (Entity is null && Dto is null)
//        {
//            result.Message = $"{nameof(Entity)} && {nameof(Dto)} is null {nameof(AddUpdateCommandBase)}";
//            result.IsSuccess = false;
//            return result;
//        }
//        _apiIdentity = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IApiIdentity>();
//        _mapper = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IMapper>();

//        if (Entity == null)
//            Entity = _mapper.Map<EntityBase>(Dto);

//        if (request.IsUpdate && request.IgnoreCheckIfEntityExist is false)
//        {
//            var repository = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IRepository<EntityBase>>();
//            var repo = await repository.GetId(Entity.Id);
//            if (repo == null)
//            {
//                result.Message = $"{nameof(Entity.Id)} {Entity.Id} not found";
//                result.IsSuccess = false;
//                result.StatusCode = StatusCodes.Status404NotFound;
//                return result;
//            }
//            repo = _mapper.Map(Entity, repo);
//            Entity = repo;
//        }

//        if (request.IsUpdate && request.IgnoreValidateCanModify is false)
//        {
//            if (await CanModify(Entity) is false)
//            {
//                result.Message = $"User not allowed to modify {nameof(Entity)}";
//                result.IsSuccess = false;
//                result.StatusCode = StatusCodes.Status405MethodNotAllowed;
//                return result;
//            }
//        }
//        //if (request.IsUpdate)
//        //    Entity.AddDomainEvent(new UpdatedEvent<TEntity>(entity));


//        return result;
//    }

//    /// <summary>
//    /// Some Optional validation for Write Operations.
//    /// Must override CanModifyFunc
//    /// </summary>
//    /// <param name="obj"></param>
//    /// <returns></returns>
//    protected async virtual Task<bool> CanModify(EntityBase entity, Func<EntityBase, Task<bool>> taskFunc = null)
//    {
//        if (entity == null)
//            return false;
//        if (taskFunc != null)
//        {
//            var res = await taskFunc(entity);
//            return res;
//        }
//        return true;
//    }


//    /// <summary>
//    /// Only Admin or user who created can modify record
//    /// To add custom execution, validation must override this.    
//    /// </summary>
//    /// <param name="entity"></param>
//    /// <returns></returns>
//    protected virtual Task<bool> CanModifyFunc(EntityBase entity)
//    {
//        //#if DEBUG
//        //        //TODO fix this
//        //        return Task.FromResult(true);
//        //#endif
//        return _apiIdentity.CanModifyFuncAsync(entity.CreatedBy);
//    }

//}
