using Server.Api.Contracts.Requests;
using Server.Domain.Models;

namespace Server.Api.Mappers
{
    public static class GameInfoMapper
    {
        public static GameInfo ToGameInfo(this GameInfoUpdateRequest request)
        {
            return new GameInfo
            {
                Map = request.Map,
                MaxPlayers = request.MaxPlayers,
                CurrentPlayers = request.CurrentPlayers
            };
        }
    }
}
