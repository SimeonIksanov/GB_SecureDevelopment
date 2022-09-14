using CardStorageService.Data;

namespace CardStorageService.Services.Implementation
{
    public class CardRepository : ICardRepository
    {
        private readonly ILogger<CardRepository> _logger;
        private readonly CardStorageServiceDbContext _context;
        #region Constructors

        public CardRepository(
            ILogger<CardRepository> logger,
            CardStorageServiceDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        #endregion
        public Guid Create(Card entity)
        {
            var client = _context.Clients.Find(entity.ClientId);
            if (client == null)
                throw new Exception("Client not found");
            _context.Cards.Add(entity);
            _context.SaveChanges();
            return entity.CardId;
        }

        public int Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public IList<Card> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<Card> GetByClientId(int id)
        {
            throw new NotImplementedException();
        }

        public Card GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public int Update(Card entity)
        {
            throw new NotImplementedException();
        }
    }
}
