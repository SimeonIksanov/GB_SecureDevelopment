using CardStorageService.Data;

namespace CardStorageService.Services.Implementation;

public class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
{
    protected readonly CardStorageServiceDbContext _context;

    #region Constructors

    public Repository(CardStorageServiceDbContext context)
    {
        _context = context;
    }

    #endregion

    public virtual TId Create(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
        _context.SaveChanges();
        return entity.Id;
    }

    public virtual int Delete(TId id)
    {
        var entity = _context.Set<TEntity>().Find(id);
        if (entity == null)
            return 0;
        _context.Set<TEntity>().Remove(entity);
        return _context.SaveChanges();
    }

    public virtual IList<TEntity> GetAll()
    {
        return _context.Set<TEntity>().ToList();
    }

    public virtual TEntity GetById(TId id)
    {
        return _context.Set<TEntity>().Find(id);
    }

    public virtual int Update(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        return _context.SaveChanges();
    }
}
