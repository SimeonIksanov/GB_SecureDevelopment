using Microsoft.AspNetCore.Mvc;

namespace CardStorageService.Controllers;

[Route("api/[controller]")]
[ApiController]

public class CardController : ControllerBase
{
    #region Services
    
    private readonly ILogger<CardController> _logger;

    #endregion

    #region Constructors
    public CardController(ILogger<CardController> logger)
    {
        _logger = logger;
    }

    #endregion

    #region Public Methods

    [HttpGet("getAll")]
    public IActionResult GetByClientId()
    {
        _logger.LogInformation("I am in GetByClientId..");
        return Ok();
    }
    #endregion
}
