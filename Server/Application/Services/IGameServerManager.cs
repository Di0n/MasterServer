using Microsoft.AspNetCore.Hosting.Server;
using Server.Api.DTOs;
using Server.Domain.Models;
using System.Net.WebSockets;
using System.Security.Cryptography;

namespace Server.Application.Services
{

    public interface IGameServerManager
    {
        public GameServer RegisterServer(string name, string host, uint port, WebSocket connection);
        public void UnregisterServer(string serverId);
        public void UnregisterServer(WebSocket socket);
        public void UnregisterServer(GameServer server);

        public IEnumerable<GameServerDto> GetActiveServers();

        public bool ValidateAuthToken(string serverId, string token);

        public void UpdateServerStatus(string serverId, GameInfo update);
        public void UpdateServerStatus(WebSocket socket, GameInfo update);
        public void UnregisterInactiveServers();
        public GameServer? GetServer(string serverId);
        public GameServer? GetServer(WebSocket socket);
    }
}
