//using MassTransit;

//namespace CommonBE.Infrastructure.Persistence;

//public interface IRepositoryCtx<TModel, TDbContext>
//where TModel : EntityBase
//where TDbContext : DbContext
//{
//    IApiIdentity ApiIdentity { get; }
//    TDbContext DatabaseContext { get; }
//    IDateTimeService DateTimeService { get; }

//    void Add(TModel entity, string UserId = null);
//    Task<TModel> AddAsync(TModel entity, string UserId = null);
//    void AddRange(IEnumerable<TModel> entitys, string UserId = null);
//    Task AddRangeAsync(IEnumerable<TModel> entitys, string UserId = null);
//    Task<int> CountAsync();
//    Task<List<TModel>> GetByUserId(string userId);
//    Task<TModel> GetFilter(Expression<Func<TModel, bool>> expression);
//    Task<TModel> GetFilter(Expression<Func<TModel, bool>> expression, params Expression<Func<TModel, object>>[] includes);
//    Task<TModel> GetId(string id);
//    Task<List<TModel>> GetList();
//    Task<List<EntitySoftDeleteBase>> GetListActive(params Expression<Func<EntitySoftDeleteBase, object>>[] includes);
//    Task<List<TModel>> GetListFilter(Expression<Func<TModel, bool>> expression);
//    Task<List<TModel>> GetListFilter(Expression<Func<TModel, bool>> expression, params Expression<Func<TModel, object>>[] includes);
//    void Remove(TModel entity, string UserId = null);
//    Task<bool> RemoveAsync(string Id, string UserId = null);
//    void ResetAtByUser(TModel entity);
//    Task<bool> SaveChangesAsync();
//    void SetAddProperties(TModel entity, string UserId);
//    void Update(TModel entity, string UserId = null);
//    Task<TModel> UpdateAsync(TModel entity, string UserId = null);
//}

//public class RepositoryCtx<TModel, TDbContext> : IRepositoryCtx<TModel, TDbContext> where TModel : EntityBase
//                                               where TDbContext : DbContext
//{
//    public TDbContext DatabaseContext { get; }
//    public IApiIdentity ApiIdentity { get; }
//    public IDateTimeService DateTimeService { get; }

//    public RepositoryCtx(TDbContext context, IApiIdentity apiIdentity, IDateTimeService dateTimeService)
//    {
//        DatabaseContext = context ?? throw new ArgumentNullException(nameof(context));
//        ApiIdentity = apiIdentity ?? throw new ArgumentNullException(nameof(apiIdentity));
//        DateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
//    }

//    public virtual void Add(TModel entity, string UserId = null)
//    {
//        SetAddProperties(entity, UserId);
//        if (entity is IEntityEmailBase)
//        {
//            var email = ApiIdentity.GetCurrentUserEmail().Result; //it is fine, method is synch anyway                                
//            var casted = entity as IEntityEmailBase;
//            if (string.IsNullOrWhiteSpace(casted?.Email)) //Email.IsNullOrEmptyExt())
//                casted.Email = email;
//        }
//        DatabaseContext.Set<TModel>().Add(entity);
//    }

//    public void AddRange(IEnumerable<TModel> entitys, string UserId = null)
//    {
//        foreach (var entity in entitys)
//            SetAddProperties(entity, UserId);
//        DatabaseContext.Set<TModel>().AddRange(entitys);
//    }

//    public virtual void SetAddProperties(TModel entity, string UserId)
//    {
//        //if (entity.Id != 0)
//        //    throw new ArgumentException($"Id {entity.Id} can not be set while add operation.");        
//        if (string.IsNullOrWhiteSpace(entity.Id) || Guid.Parse(entity.Id) == Guid.Empty)
//            //entity.Id = Guid.NewGuid().ToString();            
//            entity.Id = NewId.Next().ToString();

//        entity.CreatedBy = UserId ?? ApiIdentity.GetUserNameOrIp();
//        entity.CreatedAt ??= DateTimeService.UtcNow;
//    }

//    public virtual void Update(TModel entity, string UserId = null)
//    {
//        DatabaseContext.Entry(entity).State = EntityState.Modified;
//        entity.ModifiedBy = UserId ?? ApiIdentity.GetUserNameOrIp();
//        entity.ModifiedAt = DateTimeService.UtcNow;
//    }

//    public virtual async Task<TModel> AddAsync(TModel entity, string UserId = null)
//    {
//        Add(entity, UserId);
//        await SaveChangesAsync();
//        return entity;
//    }

//    public virtual async Task AddRangeAsync(IEnumerable<TModel> entitys, string UserId = null)
//    {
//        foreach (var entity in entitys)
//            SetAddProperties(entity, UserId);
//        DatabaseContext.Set<TModel>().AddRange(entitys);
//        await SaveChangesAsync();
//    }

//    public virtual async Task<TModel> UpdateAsync(TModel entity, string UserId = null)
//    {
//        Update(entity, UserId);
//        await SaveChangesAsync();
//        return entity;
//    }

//    public virtual void Remove(TModel entity, string UserId = null)
//    {
//        if (entity is EntitySoftDeleteBase)
//        {
//            (entity as EntitySoftDeleteBase).IsDeleted = true;
//            Update(entity, UserId ?? ApiIdentity.GetUserNameOrIp());
//        }
//        else
//        {
//            DatabaseContext.Entry(entity).State = EntityState.Deleted;
//            DatabaseContext.Set<TModel>().Remove(entity);
//        }
//    }

//    public virtual async Task<bool> RemoveAsync(string Id, string UserId = null)
//    {
//        if (string.IsNullOrWhiteSpace(Id))
//            throw new ArgumentException($"Id can not be null or empty.");
//        var entity = await GetId(Id).ConfigureAwait(false);
//        if (entity is null)
//            throw new ArgumentException($"Id can not be found.");

//        Remove(entity, UserId);
//        return await SaveChangesAsync().ConfigureAwait(false);
//    }

//    public virtual Task<TModel> GetFilter(Expression<Func<TModel, bool>> expression, params Expression<Func<TModel, object>>[] includes)
//    {
//        DbSet<TModel> dbSet = DatabaseContext.Set<TModel>();
//        IQueryable<TModel> query = null;
//        foreach (var includeExpression in includes)
//            query = dbSet.Include(includeExpression);
//        return query.AsNoTracking().FirstOrDefaultAsync(expression);
//    }

//    public virtual Task<TModel> GetFilter(Expression<Func<TModel, bool>> expression) =>
//        DatabaseContext.Set<TModel>().AsNoTracking().FirstOrDefaultAsync(expression);

//    public virtual Task<List<TModel>> GetList() => DatabaseContext.Set<TModel>().AsNoTracking().OrderByDescending(a => a.CreatedAt).ToListAsync();

//    //public virtual async Task<TModel> GetId(string id) => await DatabaseContext.Set<TModel>().FindAsync(id).ConfigureAwait(false);
//    public virtual Task<TModel> GetId(string id) => DatabaseContext.Set<TModel>().AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

//    public virtual Task<List<TModel>> GetByUserId(string userId)
//        => DatabaseContext.Set<TModel>().AsNoTracking().Where(a => a.CreatedBy.Contains(userId)).OrderByDescending(a => a.CreatedAt).ToListAsync();

//    public virtual Task<List<TModel>> GetListFilter(Expression<Func<TModel, bool>> expression)
//        => DatabaseContext.Set<TModel>().AsNoTracking().Where(expression).OrderByDescending(a => a.CreatedAt).ToListAsync();

//    public virtual Task<List<TModel>> GetListFilter(Expression<Func<TModel, bool>> expression, params Expression<Func<TModel, object>>[] includes)
//    {
//        DbSet<TModel> dbSet = DatabaseContext.Set<TModel>();
//        IQueryable<TModel> query = null;
//        foreach (var includeExpression in includes)
//            query = dbSet.Include(includeExpression);
//        return query.AsNoTracking().Where(expression).ToListAsync();
//    }

//    public virtual Task<List<EntitySoftDeleteBase>> GetListActive(params Expression<Func<EntitySoftDeleteBase, object>>[] includes)
//    {
//        DbSet<EntitySoftDeleteBase> dbSet = DatabaseContext.Set<EntitySoftDeleteBase>();
//        IQueryable<EntitySoftDeleteBase> query = dbSet;
//        foreach (var includeExpression in includes)
//            query = dbSet.Include(includeExpression);
//        query = query.Where(a => a.IsDeleted != true);
//        return query.AsNoTracking().ToListAsync();
//    }

//    public virtual async Task<bool> SaveChangesAsync()
//    {
//        try
//        {
//            var saved = await DatabaseContext.SaveChangesAsync().ConfigureAwait(false) >= 0;
//            return saved;
//        }
//        catch (DbUpdateException e)
//        {
//            // get latest version of record for display
//            //throw new DbUpdateException("Conflict detected, refresh and try again.");
//            //if (e.InnerException != null && e.InnerException.Message.Contains("when IDENTITY_INSERT is set to OFF"))
//            //    return true;
//            throw new DbUpdateException(e?.Message);
//        }
//        catch (Exception ee)
//        {
//            return false;
//        }
//    }

//    public void ResetAtByUser(TModel entity)
//    {
//        if (entity == null)
//            return;
//        var b = entity as EntityBase;
//        if (b == null)
//            return;

//        b.CreatedAt = null;
//        b.CreatedBy = null;
//        b.ModifiedAt = null;
//        b.ModifiedBy = null;
//        if (entity is EntitySoftDeleteBase)
//            (entity as EntitySoftDeleteBase).Email = null;
//    }

//    public Task<int> CountAsync() => DatabaseContext.Set<TModel>().AsNoTracking().CountAsync();

//}
