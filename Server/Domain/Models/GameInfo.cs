namespace Server.Domain.Models
{
    public class GameInfo
    {
        public string Map { get; set; } = "";
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }

        public override string ToString()
        {
            return $"{CurrentPlayers}/{MaxPlayers} on {Map}";
        }
    }
}
