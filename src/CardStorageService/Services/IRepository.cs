namespace CardStorageService.Services;

public interface IRepository<TEntity, TId>
{
    IList<TEntity> GetAll();

    TEntity GetById(TId id);

    TId Create(TEntity entity);

    int Update(TEntity entity);

    int Delete(TId id);
}
