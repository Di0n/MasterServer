using System.Net.WebSockets;

namespace Server.Application.Services
{
    public interface IServerCommService
    {
        public Task<bool> DoHandshake(WebSocket socket);
        public Task RegisterServer(WebSocket socket);
        public Task StartHeartbeatComm(WebSocket socket);
    }
}
