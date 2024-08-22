using CommonCleanArch.Domain.Base;

namespace CommonCleanArch.Infrastructure.Persistence;

//public interface IQuery<TResponse> : IRequest<Result<TResponse>>
//{

//}

//public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
//    where TQuery : IQuery<TResponse>
//{

//}

//public interface IQueryId<TResponse> : IQuery<Result<TResponse>>
//{
//    string Id { get; }
//}

//public class QueryId<TResponse> : IQueryId<TResponse>
//{
//    public required string Id { get; set; }
//}

//public interface IQueryIdHandler<TQuery, TResponse> : IQueryHandler<TQuery, Result<TResponse>>
//    where TQuery : IQueryId<TResponse>
//{
//}

////public class QueryIdHandler<TEntity> : IQueryIdHandler<QueryId, TEntity> where TEntity : EntityBase
////{
////    public QueryIdHandler()
////    {
////    }
////}


public interface IQuery<TResponse> : IRequest<TResponse>
{

}

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}

public interface IQueryId<TEntity> : IQuery<TEntity> where TEntity : IEntity
{
    string Id { get; }
}

public class QueryId<TEntity> : IQueryId<TEntity> where TEntity : IEntity
{
    public required string Id { get; set; }
}

//public interface IQueryIdHandler<TQuery, TEntity> : IQueryHandler<TQuery, IEntity>
//    where TQuery : IQueryId<TEntity> where TEntity : IEntity
//{

//}

//public class QueryIdHandler<TEntity> : IQueryIdHandler<QueryId, TEntity> where TEntity : EntityBase
//{
//    public QueryIdHandler()
//    {
//    }

//    public Task<TEntity> Handle(QueryId request, CancellationToken cancellationToken)
//    {
//        throw new NotImplementedException();
//    }
//}
