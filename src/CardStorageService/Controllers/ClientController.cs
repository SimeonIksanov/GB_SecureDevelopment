using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CardStorageService.Services;
using CardStorageService.Models.Requests;
using CardStorageService.Data;
using Microsoft.AspNetCore.Authorization;
using FluentValidation.Results;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace CardStorageService.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    #region Services
    
    private readonly ILogger<ClientsController> _logger;
    private readonly IClientRepository _clientRepository;
    private readonly IValidator<ClientCreateRequest> _clientCreateRequestValidator;

    #endregion

    #region Constructors
    public ClientsController(ILogger<ClientsController> logger,
                             IClientRepository clientRepository,
                             IValidator<ClientCreateRequest> clientCreateRequestValidator)
    {
        _logger = logger;
        _clientRepository = clientRepository;
        _clientCreateRequestValidator = clientCreateRequestValidator;
    }

    #endregion

    #region Public Methods

    [HttpPost()]
    [ProducesResponseType(typeof(ClientCreateResponse), StatusCodes.Status200OK)]
    public IActionResult Create([FromBody]ClientCreateRequest request)
    {
        ValidationResult validationResult = _clientCreateRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        try
        {
            var clientId = _clientRepository.Create(new Client
            {
                FirstName = request.FirstName,
                Patronymic = request.Patronymic,
                Surname = request.Surname,
            });
            return Ok(new ClientCreateResponse
            {
                ClientId = clientId
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Create client error");
            return Ok(new ClientCreateResponse
            {
                ErrorCode = 912,
                ErrorMessage = "Create client error"
            });
        }
    }
    #endregion
}