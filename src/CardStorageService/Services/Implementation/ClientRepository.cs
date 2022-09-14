using CardStorageService.Data;

namespace CardStorageService.Services.Implementation
{
    public class ClientRepository : IClientRepository
    {
        private readonly ILogger<ClientRepository> _logger;
        private readonly CardStorageServiceDbContext _context;

        #region Constructors

        public ClientRepository(
            ILogger<ClientRepository> logger,
            CardStorageServiceDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        #endregion

        public int Create(Client entity)
        {
            _context.Clients.Add(entity);
            _context.SaveChanges();
            return entity.ClientId;
        }

        public int Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IList<Client> GetAll()
        {
            throw new NotImplementedException();
        }

        public Client GetById(int id)
        {
            throw new NotImplementedException();
        }

        public int Update(Client entity)
        {
            throw new NotImplementedException();
        }
    }
}
