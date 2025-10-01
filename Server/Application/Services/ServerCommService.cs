using Server.Api.Contracts.Requests;
using Server.Api.Contracts.Responses;
using Server.Api.Mappers;
using Server.Utils;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Server.Application.Services
{
    public class ServerCommService : IServerCommService
    {
        private readonly IGameServerManager manager;
        private readonly IConfiguration configuration;
        private readonly double heartbeatInterval;
        private readonly ILogger<ServerCommService> logger;
        public ServerCommService(IGameServerManager manager, IConfiguration configuration, ILogger<ServerCommService> logger)
        {
            this.manager = manager;
            this.configuration = configuration;
            this.logger = logger;
            heartbeatInterval = configuration.GetValue<double>("Server:HeartbeatInterval");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public async Task<bool> DoHandshake(WebSocket socket)
        {
            string challengeToken = AuthUtils.GenerateToken(16);

            var request = new HandshakeRequest() { Challenge = $"{challengeToken}" };
            var msg = new WebSocketMessage
            { 
                Type = nameof(HandshakeRequest),
                Payload = JsonSerializer.SerializeToElement<HandshakeRequest>(request)
            };

            DateTimeOffset handshakeStart = DateTimeOffset.UtcNow;
            await SendMessageAsync(socket, msg, CancellationToken.None);
            
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var response = await ReceiveMessageAsync(socket, cts.Token);

            DateTimeOffset handshakeEnd = DateTimeOffset.UtcNow;
            if (!WebSocketMessage.IsValid(response))
            {
                logger.LogError("Invalid handshake response");
                return false;
            }
            
            if (response?.Type != nameof(HandshakeResponse))
            {
                logger.LogError("Unexpected message type during handshake: {Type}", response?.Type);
                return false;
            }

            var hResp = msg?.Payload.Deserialize<HandshakeResponse>();

            //string[] responseParts = hResp!.Response.Split(':');
            //if (responseParts.Length < 2)
            //{
            //    logger.LogError("Invalid handshake response data");
            //    return false;
            //}

            //if (!long.TryParse(responseParts[1], out long responseTime))
            //{
            //    logger.LogError("Invalid handshake response data type");
            //    return false;
            //}
            
            // Prevent replay attacks
            TimeSpan timeDiff = handshakeEnd - handshakeStart;
            double maxTimeDiff = 3;

            if (challengeToken == hResp?.Response && timeDiff.TotalSeconds < maxTimeDiff)
            {
                // TODO add to approved connections?
                return true;
            }
            else
            {
                logger.LogError($"Handshake failed, received: {hResp?.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public async Task StartHeartbeatComm(WebSocket socket)
        {
            while (socket.State == WebSocketState.Open)
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(heartbeatInterval));

                WebSocketMessage? msg = await ReceiveMessageAsync(socket, cts.Token);

                if (msg == null) continue;

                switch (msg.Type)
                {
                    case nameof(GameInfoUpdateRequest):
                        var gameInfoUpdate = msg.Payload.Deserialize<GameInfoUpdateRequest>();
                        if (gameInfoUpdate != null)
                            manager.UpdateServerStatus(socket, GameInfoMapper.ToGameInfo(gameInfoUpdate));
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public async Task RegisterServer(WebSocket socket)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(heartbeatInterval));

            var msg = await ReceiveMessageAsync(socket, cts.Token);

            if (msg == null)
                throw new JsonException("Received null message during registration");

            if (msg?.Type != nameof(RegisterServerRequest))
            {
                throw new InvalidDataException($"Type is not {nameof(RegisterServerRequest)}");
            }

            var registerServerRequest = msg?.Payload.Deserialize<RegisterServerRequest>();
            

            GameServer server = manager.RegisterServer(registerServerRequest!.Name, registerServerRequest.Host, registerServerRequest.Port, socket);

            var response = new RegisterServerResponse()
            {
                ServerID = server.Id,
                AuthToken = server.AuthToken
            };

            var responseMsg = new WebSocketMessage
            {
                Type = nameof(RegisterServerResponse),
                Payload = JsonSerializer.SerializeToElement<RegisterServerResponse>(response)
            };

            await SendMessageAsync(socket, responseMsg, CancellationToken.None);
        }

        private Task SendMessageAsync(WebSocket socket, WebSocketMessage message, CancellationToken cancellationToken)
        {
            string jsonStr = JsonSerializer.Serialize(message);
            byte[] buffer = Encoding.UTF8.GetBytes(jsonStr);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, cancellationToken);
        }
        private async Task<WebSocketMessage?> ReceiveMessageAsync(WebSocket socket, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 4];
            var result = new ArraySegmentBuffer();

            WebSocketReceiveResult? receiveResult;
            do
            {
                receiveResult = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cancellationToken);

                if (receiveResult.MessageType == WebSocketMessageType.Close)
                    return null;

                result.Append(new ArraySegment<byte>(buffer, 0, receiveResult.Count));
            }
            while (!receiveResult?.EndOfMessage ?? false);

            string jsonStr = Encoding.UTF8.GetString(result.Buffer.ToArray());
            
            return JsonSerializer.Deserialize<WebSocketMessage>(jsonStr);
        }

        // Helper class to handle growing buffer
        private class ArraySegmentBuffer
        {
            private readonly List<byte> buffer = new();
            public IEnumerable<byte> Buffer => buffer;

            public void Append(ArraySegment<byte> segment)
            {
                buffer.AddRange(segment);
            }
        }

        private record WebSocketMessage
        {
            public string Type { get; set; } = "";
            public JsonElement Payload { get; set; }

            public static bool IsValid(WebSocketMessage? wsm) => wsm != null && !string.IsNullOrEmpty(wsm.Type);
        }
    }
}
