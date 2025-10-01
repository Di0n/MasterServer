namespace Server.Api.Contracts.Responses
{
    public record HandshakeResponse
    {
        public required string Response { get; set; }

        public override string ToString()
        {
            return Response;
        }
    }
}
