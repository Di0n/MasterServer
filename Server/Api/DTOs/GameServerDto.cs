using Server.Domain.Models;

namespace Server.Api.DTOs
{
    public class GameServerDto
    {
        public static GameServerDto ToDto(GameServer gameServer)
        {
            return new GameServerDto
            {
                Name = gameServer.Name,
                Host = gameServer.Host,
                Port = gameServer.Port,
                Info = gameServer.Info
            };
        }

        public string Name { get; set; } = "";
        public string Host { get; set; } = "";
        public uint Port { get; set; }

        public GameInfo? Info { get; set; }
    }
}
