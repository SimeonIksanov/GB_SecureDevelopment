using AutoMapper;
using CardStorageService.Controllers;
using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Requests;
using CardStorageServiceProtos;
using FluentValidation;
using Grpc.Core;
using static CardStorageServiceProtos.CardService;

namespace CardStorageService.Services.Implementation;

public class CardService : CardServiceBase
{
    #region Services

    private readonly ILogger<CardsController> _logger;
    private readonly ICardRepository _cardRepository;
    private readonly IValidator<Models.Requests.CardCreateRequest> _cardCreateRequestValidator;
    private readonly IMapper _mapper;

    #endregion

    #region Constructors
    public CardService(ILogger<CardsController> logger,
                       ICardRepository cardRepository,
                       IValidator<Models.Requests.CardCreateRequest> cardCreateRequestValidator,
                       IMapper mapper)
    {
        _logger = logger;
        _cardRepository = cardRepository;
        _cardCreateRequestValidator = cardCreateRequestValidator;
        _mapper = mapper;
    }

    #endregion


    public override Task<CardStorageServiceProtos.CardCreateResponse> Create(CardStorageServiceProtos.CardCreateRequest request, ServerCallContext context)
    {
        var cardId = _cardRepository.Create(_mapper.Map<Data.Card>(request));
        var response = new CardStorageServiceProtos.CardCreateResponse { CardId = cardId.ToString() };
        return Task.FromResult(response);
    }

    public override Task<GetByClientIdResponse> GetByClientId(GetByClientIdRequest request, ServerCallContext context)
    {
        if (request.ClientId <= 0)
        {
            var meta = new Metadata();
            meta.Add("ClientId", "Must be greater then 0");
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Failed to validate ClientId"), meta);
        }

        var cards = _cardRepository.GetByClientId(request.ClientId);
        var response = new GetByClientIdResponse();
        response.Cards.AddRange(_mapper.Map<List<CardStorageServiceProtos.Card>>(cards));

        return Task.FromResult(response);
    }
}
