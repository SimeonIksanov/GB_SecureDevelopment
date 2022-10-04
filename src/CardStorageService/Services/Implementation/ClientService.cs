using AutoMapper;
using CardStorageService.Controllers;
using CardStorageService.Data;
using CardStorageService.Models.Validators;
using CardStorageServiceProtos;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Grpc.Core;
using static CardStorageServiceProtos.ClientService;

namespace CardStorageService.Services.Implementation;

public class ClientService : ClientServiceBase
{
    #region Services

    private readonly ILogger<ClientService> _logger;
    private readonly IClientRepository _clientRepository;
    private readonly IValidator<ClientCreateRequest> _clientCreateRequestValidator;
    private readonly IMapper _mapper;

    #endregion

    #region Constructors

    public ClientService(ILogger<ClientService> logger,
                         IClientRepository clientRepository,
                         IValidator<ClientCreateRequest> clientCreateRequestValidator,
                         IMapper mapper)
    {
        _logger = logger;
        _clientRepository = clientRepository;
        _clientCreateRequestValidator = clientCreateRequestValidator;
        _mapper = mapper;
    }

    #endregion
    
    
    public override Task<ClientCreateResponse> Create(ClientCreateRequest request, ServerCallContext context)
    {
        ValidationResult validationResult = _clientCreateRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "validation failure"));
        }

        int clientId = _clientRepository.Create(_mapper.Map<Client>(request));
        ClientCreateResponse clientCreateResponse = new ClientCreateResponse
        {
            ClientId = clientId
        };
        context.Status = new Status();
        return Task.FromResult(clientCreateResponse);
    }
}