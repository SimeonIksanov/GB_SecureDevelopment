using AutoMapper;
using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Requests;
using CardStorageService.Models.Validators;
using CardStorageService.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardStorageService.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CardsController : ControllerBase
{
    #region Services
    
    private readonly ILogger<CardsController> _logger;
    private readonly ICardRepository _cardRepository;
    private readonly IValidator<CardCreateRequest> _cardCreateRequestValidator;
    private readonly IMapper _mapper;

    #endregion

    #region Constructors
    public CardsController(ILogger<CardsController> logger,
                           ICardRepository cardRepository,
                           IValidator<CardCreateRequest> cardCreateRequestValidator,
                           IMapper mapper)
    {
        _logger = logger;
        _cardRepository = cardRepository;
        _cardCreateRequestValidator = cardCreateRequestValidator;
        _mapper = mapper;
    }

    #endregion

    #region Public Methods

    [HttpPost()]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult Create([FromBody]CardCreateRequest request)
    {
        ValidationResult validationResult = _cardCreateRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        try
        {
            var cardid = _cardRepository.Create(_mapper.Map<Card>(request));
            return Ok(new CardCreateResponse
            {
                CardId = cardid.ToString()
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Create card error");
            return Ok(new CardCreateResponse
            {
                ErrorCode = 1012,
                ErrorMessage = "Create card error"
            });
        }
    }

    [HttpGet()]
    [ProducesResponseType(typeof(GetCardsResponse), StatusCodes.Status200OK)]
    public IActionResult GetByClientId([FromQuery]int clientId)
    {
        try
        {
            var cards = _cardRepository.GetByClientId(clientId);
            return Ok(new GetCardsResponse
            {
                Cards = _mapper.Map<List<CardDto>>(cards)
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get cards error.");
            return Ok(new GetCardsResponse
            {
                ErrorCode = 1013,
                ErrorMessage = "Get cards error."
            });
        }
    }
    #endregion
}
