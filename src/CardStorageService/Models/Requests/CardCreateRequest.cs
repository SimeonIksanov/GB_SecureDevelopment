namespace CardStorageService.Models.Requests
{
    public class CardCreateRequest
    {
        public int ClientId { get; set; }

        public string CardNo { get; set; }

        public string? Name { get; set; }

        public string? CVV2 { get; set; }

        public DateTime ExpireDate { get; set; }
    }
}
