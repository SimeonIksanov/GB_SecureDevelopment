using AutoMapper;
using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Requests;

namespace CardStorageService.Mappings;

public class MappingsProfile : Profile
{
    public MappingsProfile()
    {
        CreateMap<CardCreateRequest, Card>();
        CreateMap<Card, CardDto>();
        CreateMap<ClientCreateRequest, Client>();
        CreateMap<Card, CardStorageServiceProtos.Card>();
        CreateMap<CardStorageServiceProtos.CardCreateRequest, Card>();
    }
}
