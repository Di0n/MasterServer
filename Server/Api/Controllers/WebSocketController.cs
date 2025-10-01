using Microsoft.AspNetCore.Mvc;
using Server.Application.Services;
using System.Net.WebSockets;

namespace Server.Api.Controllers
{
    [ApiController]
    [Route("ws")]
    public class WebSocketController : ControllerBase
    {
        private readonly IServerCommService handler;
        private readonly ILogger<WebSocketController> logger;

        public WebSocketController(IServerCommService handler, ILogger<WebSocketController> logger)
        {
            this.handler = handler;
            this.logger = logger;
        }

        [HttpGet("connect")]
        public async Task Get()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                logger.LogWarning("Received non-WebSocket request on WebSocket endpoint");
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }    
            // if not banned
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            // Connection accepted

            if (! await handler.DoHandshake(webSocket))
            {
                logger.LogError("Handshake failed, closing connection");
                await webSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Handshake failed", CancellationToken.None);
                return;
            }

            // TODO try catch
            await handler.RegisterServer(webSocket);

            await handler.StartHeartbeatComm(webSocket); // long running

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "No more hearbeats, closing..", CancellationToken.None);
        }
    }
}
