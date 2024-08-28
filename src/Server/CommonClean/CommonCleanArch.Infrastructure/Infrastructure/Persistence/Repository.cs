using CommonCleanArch.Application.Services;
using Microsoft.EntityFrameworkCore.Storage;

namespace CommonCleanArch.Infrastructure.Persistence;


public class Repository<TEntity> : IDisposable, IRepository<TEntity> where TEntity : EntityBase
{
    public DbContext DatabaseContext { get; }
    public IApiIdentity ApiIdentity { get; }
    public IDateTimeService DateTimeService { get; }

    public Repository(DbContext context, IApiIdentity apiIdentity, IDateTimeService dateTimeService)
    {
        DatabaseContext = context ?? throw new ArgumentNullException(nameof(context));
        ApiIdentity = apiIdentity ?? throw new ArgumentNullException(nameof(apiIdentity));
        DateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
    }

    private IDbContextTransaction? _CurrentTransaction;
    public virtual IDbContextTransaction GetTransaction(bool newTransaction = false)
    {
        if (newTransaction)
        {
            _CurrentTransaction?.Dispose();
            _CurrentTransaction = DatabaseContext.Database.BeginTransaction();
            return _CurrentTransaction;
        }
        if (_CurrentTransaction is not null)
            return _CurrentTransaction;
        _CurrentTransaction = DatabaseContext.Database.BeginTransaction();
        return _CurrentTransaction;
    }

    public virtual void UseTransaction(IDbContextTransaction transaction)
    {
        _CurrentTransaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        DatabaseContext.Database.UseTransaction(transaction.GetDbTransaction());
    }

    //public virtual bool CommitTransaction()
    //{
    //    try
    //    {
    //        _CurrentTransaction?.Commit();
    //        return true;
    //    }
    //    catch
    //    {
    //        _CurrentTransaction?.Rollback();                        
    //    }
    //    finally
    //    {
    //        _CurrentTransaction?.Dispose();
    //        _CurrentTransaction = null;            
    //    }
    //    return false;
    //}

    public virtual void Add(TEntity entity, string UserId = null)
    {
        SetAddProperties(entity, UserId);
        if (entity is IEntityEmailBase)
        {
            var email = ApiIdentity.GetCurrentUserEmail().Result; //it is fine, method is synch anyway                                
            var casted = entity as IEntityEmailBase;
            if (string.IsNullOrWhiteSpace(casted?.Email)) //Email.IsNullOrEmptyExt())
                casted.Email = email;
        }
        DatabaseContext.Set<TEntity>().Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entitys, string UserId = null)
    {
        foreach (var entity in entitys)
            SetAddProperties(entity, UserId);
        DatabaseContext.Set<TEntity>().AddRange(entitys);
    }

    public virtual void SetAddProperties(TEntity entity, string UserId)
    {
        //if (entity.Id != 0)
        //    throw new ArgumentException($"Id {entity.Id} can not be set while add operation.");        
        //if (string.IsNullOrWhiteSpace(entity.Id) || Guid.Parse(entity.Id) == Guid.Empty)
        if (entity is not null && entity.Id.IsNullNotValidIdExt())
            //entity.Id = Guid.NewGuid().ToString();            
            entity.Id = StringHelper.GetStringGuidExt();
        entity.CreatedBy = UserId ?? ApiIdentity.GetUserNameOrIp();
        entity.CreatedAt ??= DateTimeService.UtcNow;
    }

    public virtual void Update(TEntity entity, string UserId = null)
    {
        DatabaseContext.Entry(entity).State = EntityState.Modified;
        entity.ModifiedBy = UserId ?? ApiIdentity.GetUserNameOrIp();
        entity.ModifiedAt = DateTimeService.UtcNow;
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, string UserId = null)
    {
        Add(entity, UserId);
        await SaveChangesAsync();
        return entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entitys, string UserId = null)
    {
        foreach (var entity in entitys)
            SetAddProperties(entity, UserId);
        DatabaseContext.Set<TEntity>().AddRange(entitys);
        await SaveChangesAsync();
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, string UserId = null)
    {
        Update(entity, UserId);
        await SaveChangesAsync();
        return entity;
    }

    public virtual void Remove(TEntity entity, string UserId = null)
    {
        if (entity is EntitySoftDeleteBase)
        {
            (entity as EntitySoftDeleteBase).IsDeleted = true;
            Update(entity, UserId ?? ApiIdentity.GetUserNameOrIp());
        }
        else
        {
            DatabaseContext.Entry(entity).State = EntityState.Deleted;
            DatabaseContext.Set<TEntity>().Remove(entity);
        }
    }

    public virtual async Task<bool> RemoveAsync(string Id, string UserId = null)
    {
        if (string.IsNullOrWhiteSpace(Id))
            throw new ArgumentException($"Id can not be null or empty.");
        var entity = await GetId(Id).ConfigureAwait(false);
        if (entity is null)
            throw new ArgumentException($"Id can not be found.");
        Remove(entity, UserId);
        return await SaveChangesAsync().ConfigureAwait(false);
    }

    public virtual Task<TEntity> GetFilter(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
    {
        DbSet<TEntity> dbSet = DatabaseContext.Set<TEntity>();
        IQueryable<TEntity> query = null;
        foreach (var includeExpression in includes)        
            query = dbSet.Include(includeExpression);        
        return query.AsNoTracking().FirstOrDefaultAsync(expression);
    }

    public virtual Task<TEntity> GetFilter(Expression<Func<TEntity, bool>> expression) =>
        DatabaseContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(expression);
    
    public virtual Task<List<TEntity>> GetList() => DatabaseContext.Set<TEntity>().AsNoTracking().OrderByDescending(a => a.CreatedAt).ToListAsync();

    public virtual Task<TEntity> GetId(string id) => DatabaseContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

    public virtual Task<List<TEntity>> GetByUserId(string userId) 
        => DatabaseContext.Set<TEntity>().AsNoTracking().Where(a => a.CreatedBy.Contains(userId)).OrderByDescending(a => a.CreatedAt).ToListAsync();

    public virtual Task<List<TEntity>> GetListFilter(Expression<Func<TEntity, bool>> expression) 
        => DatabaseContext.Set<TEntity>().AsNoTracking().Where(expression).OrderByDescending(a => a.CreatedAt).ToListAsync();
    
    public virtual Task<List<TEntity>> GetListFilter(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
    {
        DbSet<TEntity> dbSet = DatabaseContext.Set<TEntity>();
        IQueryable<TEntity> query = null;
        foreach (var includeExpression in includes)        
            query = dbSet.Include(includeExpression);       
        return query.AsNoTracking().Where(expression).ToListAsync();
    }

    public virtual Task<List<EntitySoftDeleteBase>> GetListActive(params Expression<Func<EntitySoftDeleteBase, object>>[] includes)
    {
        DbSet<EntitySoftDeleteBase> dbSet = DatabaseContext.Set<EntitySoftDeleteBase>();
        IQueryable<EntitySoftDeleteBase> query = dbSet;
        foreach (var includeExpression in includes)        
            query = dbSet.Include(includeExpression);
        query = query.Where(a => a.IsDeleted != true);
        return query.AsNoTracking().ToListAsync();
    }

    public virtual async Task<bool> SaveChangesAsync()
    {
        try
        {
            var saved = await DatabaseContext.SaveChangesAsync().ConfigureAwait(false) >= 0;
            return saved;
        }
        catch (DbUpdateException e)
        {
            // get latest version of record for display
            //throw new DbUpdateException("Conflict detected, refresh and try again.");
            //if (e.InnerException != null && e.InnerException.Message.Contains("when IDENTITY_INSERT is set to OFF"))
            //    return true;

            //throw new DbUpdateException(e?.Message);
            throw;
        }
        catch (Exception ee)
        {
            return false;
        }
    }

    public void ResetAtByUser(TEntity entity)
    {
        if (entity == null)
            return;
        var b = entity as EntityBase;
        if (b == null)
            return;

        b.CreatedAt = null;
        b.CreatedBy = null;
        b.ModifiedAt = null;
        b.ModifiedBy = null;
        if (entity is EntitySoftDeleteBase)
            (entity as EntitySoftDeleteBase).Email = null;
    }

    public Task<int> CountAsync() => DatabaseContext.Set<TEntity>().AsNoTracking().CountAsync();

    #region IDisposable

    private bool _disposed = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _CurrentTransaction?.Dispose();
                DatabaseContext.Dispose();
            }
            _disposed = true;
        }
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
