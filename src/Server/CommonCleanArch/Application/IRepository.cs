namespace CommonCleanArch.Application;

public interface IRepository<TEntity> where TEntity : class
{
    DbContext DatabaseContext { get; }
    Task<TEntity> GetId(string id);
    virtual Task<List<TEntity>> GetByUserId(string userId) { throw new NotImplementedException(); }
    Task<List<TEntity>> GetList();
    void Add(TEntity entity, string UserId = null);
    void AddRange(IEnumerable<TEntity> entitys, string UserId = null);
    void Remove(TEntity entity, string UserId = null);
    void Update(TEntity entity, string UserId = null);
    Task<TEntity> AddAsync(TEntity entity, string UserId = null);
    Task<TEntity> UpdateAsync(TEntity entity, string UserId = null);
    Task<bool> SaveChangesAsync();
    Task<TEntity> GetFilter(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes);
    Task<TEntity> GetFilter(Expression<Func<TEntity, bool>> expression);
    Task<List<TEntity>> GetListFilter(Expression<Func<TEntity, bool>> expression);
    Task<List<TEntity>> GetListFilter(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes);
    void ResetAtByUser(TEntity entity);
    Task<int> CountAsync();
    Task<bool> RemoveAsync(string Id, string UserId = null);
    Task<List<EntitySoftDeleteBase>> GetListActive(params Expression<Func<EntitySoftDeleteBase, object>>[] includes);
}
