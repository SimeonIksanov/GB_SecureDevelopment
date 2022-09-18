namespace CardStorageService.Data;

public interface IEntity<TId>
{
    TId Id { get; set; }
}
