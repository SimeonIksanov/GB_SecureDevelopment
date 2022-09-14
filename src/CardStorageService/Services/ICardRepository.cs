using CardStorageService.Data;

namespace CardStorageService.Services;

public interface ICardRepository : IRepository<Card, Guid>
{
    IList<Card> GetByClientId(int id);
}
