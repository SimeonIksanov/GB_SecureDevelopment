using CardStorageService.Data;

namespace CardStorageService.Services.Implementation;

public class ClientRepository : Repository<Client, int>, IClientRepository
{
    //private readonly ILogger<ClientRepository> _logger;

    #region Constructors

    public ClientRepository(
        //ILogger<ClientRepository> logger,
        CardStorageServiceDbContext context) : base(context)
    {
        //_logger = logger;
    }

    #endregion

}
