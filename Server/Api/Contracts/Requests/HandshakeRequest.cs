namespace Server.Api.Contracts.Requests
{
    public record HandshakeRequest
    {
        public required string Challenge { get; set; }
    }
}
