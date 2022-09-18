using CardStorageService.Data;
using Microsoft.EntityFrameworkCore;

namespace CardStorageService.Services.Implementation;

public class CardRepository : Repository<Card, Guid>, ICardRepository
{
    //private readonly ILogger<CardRepository> _logger;

    #region Constructors

    public CardRepository(
        //ILogger<CardRepository> logger,
        CardStorageServiceDbContext context) : base(context)
    {
        //_logger = logger;
    }

    #endregion

    public override Guid Create(Card entity)
    {
        var client = _context.Clients.Find(entity.ClientId);
        if (client == null)
            throw new Exception("Client not found");
        return base.Create(entity);
    }

    public IList<Card> GetByClientId(int id)
    {
        var list = _context
            .Set<Client>()
            .Include(client => client.Cards)
            .SingleOrDefault(client => client.Id == id)?
            .Cards
            .ToList();
        return list != null ? list : new List<Card>() ;
    }

}
