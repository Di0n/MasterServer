using Microsoft.AspNetCore.Mvc;
using Server.Api.Contracts.Requests;
using Server.Api.Contracts.Responses;
using Server.Api.DTOs;
using Server.Application.Services;
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("api")]
public class ServersController : ControllerBase
{
    private readonly IGameServerManager serverManager;
    private readonly ILogger logger;

    public ServersController(IGameServerManager serverManager, ILogger<ServersController> logger)
    {
        this.serverManager = serverManager;
        this.logger = logger;
    }

    [HttpGet("servers")]
    public ActionResult<IEnumerable<GameServerDto>> GetServers()
    {
        // Do filtering
        var servers = serverManager.GetActiveServers();
        return Ok(servers);
    }

    [HttpPost("register")]
    public ActionResult<RegisterServerResponse> RegisterServer([FromBody] RegisterServerRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok();
        //GameServer server = serverManager.RegisterServer(request.Name, request.Host, request.Port, null);

        //logger.LogInformation("Registered new server: {ServerId} ({Name} at {Host}:{Port})", server.Id, server.Name, server.Host, server.Port);
        //return Ok(new RegisterServerResponse { ServerID = server.Id, AuthToken = server.AuthToken });
    }
}