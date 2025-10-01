namespace Server.Api.Contracts.Requests
{
    public record GameInfoUpdateRequest
    {
        public string ServerID { get; set; } = "";
        public string AuthToken { get; set; } = "";
        public string Map { get; set; } = "";
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
    }
}
