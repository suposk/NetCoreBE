//using AutoMapper;
//using CommonCleanArch.Domain.Base;
//using CommonCleanArch.Infrastructure.Persistence;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;

//namespace CommonCleanArch.Application;

///// <summary>
///// abscract class for GetByIdQuery
///// </summary>
///// <typeparam name="TDto"></typeparam>
//public abstract class GetByIdQueryBase<TDto> : IRequest<TDto>    
//    where TDto : IDtoBase
//{
//    public required string Id { get; set; }
//    public abstract string? Name { get; }
//}

///// <summary>
///// abscract class for GetByIdQueryHandler
///// </summary>
///// <typeparam name="TEntity"></typeparam>
///// <typeparam name="TDto"></typeparam>
//public abstract class GetByIdQueryBaseHandler<TEntity, TDto>(
//            //IServiceScopeFactory serviceScopeFactory
//        ) : IRequestHandler<GetByIdQueryBase<TDto>, TDto>
//    where TEntity : IEntity
//    where TDto : IDtoBase
//{
//    //public readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
//    public IApiIdentity? _apiIdentity;
//    public IMapper? _mapper;

//    public async Task<TDto> Handle(GetByIdQueryBase<TDto> request, CancellationToken cancellationToken)
//    {
//        return default;
//    }

//    //public virtual EntityBase? Entity { get; set; }
//    //public virtual DtoBase? Dto { get; set; }


//    //public async Task<OutResult<TEntity, TDto>> Handle(GetByIdQueryBase<TEntity, TDto> request, CancellationToken cancellationToken)
//    //{
//    //    var result = new OutResult<TEntity, TDto>() { IsSuccess = false };
//    //    //Entity = request.InputParam.Entity;
//    //    //Dto = request.InputParam.Dto;

//    //    try
//    //    {
//    //        if (string.IsNullOrWhiteSpace(request.Id))
//    //        {
//    //            result.Message = $"{nameof(request.Id)} is null {request.Name}";
//    //            result.IsSuccess = false;
//    //            return result;
//    //        }

//    //        if (Entity is null && Dto is null)
//    //        {
//    //            result.Message = $"{nameof(Entity)} && {nameof(Dto)} is null {request.Name}";
//    //            result.IsSuccess = false;
//    //            return result;
//    //        }

//    //        _apiIdentity = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IApiIdentity>();
//    //        _mapper = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IMapper>();


//    //        var repository = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IRepository<EntityBase>>();
//    //        var repo = await repository.GetId(request.Id);
//    //        if (repo == null)
//    //        {
//    //            result.Message = $"{request.Id} not found";
//    //            result.IsSuccess = false;
//    //            result.StatusCode = StatusCodes.Status404NotFound;
//    //            return result;
//    //        }
//    //        //Dto = _mapper.Map(Dto, repo);
//    //        Entity = repo;

//    //        if (await CanRead(Entity) is false)
//    //        {
//    //            result.Message = $"User not allowed to read {nameof(Entity)}";
//    //            result.IsSuccess = false;
//    //            result.StatusCode = StatusCodes.Status405MethodNotAllowed;
//    //            return result;
//    //        }
//    //    }
//    //    catch (Exception ex)
//    //    {
//    //        result.Message = ex?.Message;
//    //        return result;
//    //    }

//    //    return result;
//    //}

//    /// <summary>
//    /// Some Optional validation for Read Operations.
//    /// Must override CanReadFunc
//    /// </summary>
//    /// <param name="obj"></param>
//    /// <returns></returns>
//    protected async virtual Task<bool> CanRead(EntityBase entity, Func<EntityBase, Task<bool>> taskFunc = null)
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
//    /// Only Admin or user who has permision can read record
//    /// To add custom execution, validation must override this. 
//    /// </summary>
//    /// <param name="entity"></param>
//    /// <returns></returns>
//    protected virtual Task<bool> CanReadFunc(EntityBase entity)
//    {
//#if DEBUG
//        //TODO fix this
//        return Task.FromResult(true);
//#endif
//        return Task.FromResult(true);
//        //return _apiIdentity.CanReadFuncAsync(entity.CreatedBy);
//    }

//}
