using CardStorageServiceProtos;
using Grpc.Net.Client;
using static CardStorageServiceProtos.CardService;
using static CardStorageServiceProtos.ClientService;

namespace CardStorageGrpcClient;

internal class Program
{
    static void Main(string[] args)
    {
        AppContext.SetSwitch("System.Net.Http.SocketHttpHandler.Http2UnencryptedSupport", true);

        using var channel = GrpcChannel.ForAddress("http://localhost:5001");

        //ClientServiceClient clientService = new ClientServiceClient(channel);
        CardServiceClient cardService = new CardServiceClient(channel);

        var response = cardService.GetByClientId(new GetByClientIdRequest { ClientId = 1 });

        foreach (var card in response.Cards)
        {
            Console.WriteLine($"Name: {card.Name}\n{card.CardNo}");
        }

        Console.ReadKey(true);
    }
}