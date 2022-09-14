using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CardStorageService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    #region Services
    
    private readonly ILogger<ClientController> _logger;

    #endregion

    #region Constructors
    public ClientController(ILogger<ClientController> logger)
    {
        _logger = logger;
    }

    #endregion
}