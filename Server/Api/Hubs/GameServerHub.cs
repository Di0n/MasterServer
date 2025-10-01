using Microsoft.AspNetCore.SignalR;
using Server.Api.Contracts.Requests;
using Server.Api.Mappers;
using Server.Application.Services;
using Server.Domain.Models;

public class GameServerHub : Hub
{
    private readonly IGameServerManager serverManager;
    private readonly ILogger logger;

    public GameServerHub(IGameServerManager serverManager, ILogger<GameServerHub> logger)
    {
        this.serverManager = serverManager;
        this.logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var authToken = Context.GetHttpContext()?.Request.Query["token"].ToString();
        var serverId = Context.GetHttpContext()?.Request.Query["serverId"].ToString();

        if (string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(serverId) ||
            !serverManager.ValidateAuthToken(serverId, authToken))
        {
            logger.LogError("Invalid authentication attempt for server {ServerId}", serverId);
            Context.Abort();
            return;
        }

        var server = serverManager.GetServer(serverId);
        logger.LogInformation("Server {ip}:{port} with id {ServerId} connected via SignalR", server!.Host, server.Port, serverId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        //serverManager.UnregisterServer();
        await base.OnDisconnectedAsync(exception);
    }

    public void UpdateStatus(GameInfoUpdateRequest req)
    {
        if (!serverManager.ValidateAuthToken(req.ServerID, req.AuthToken))
        {
            logger.LogError("Invalid authentication attempt for server {ServerId}", req.ServerID);
            Context.Abort();
            return;
            //throw new HubException("Invalid authentication");
        }

        GameInfo gameInfo = req.ToGameInfo();
        logger.LogInformation("{serverid} updated its status to: {info}", req.ServerID, gameInfo.ToString());

        serverManager.UpdateServerStatus(req.ServerID, gameInfo);
    }

    public void Unregister(string serverId, string authToken)
    {
        if (!serverManager.ValidateAuthToken(serverId, authToken))
        {
            logger.LogError("Invalid authentication attempt for server {ServerId}", serverId);
            Context.Abort();
            return;
            //throw new HubException("Invalid authentication");
        }

        serverManager.UnregisterServer(serverId);
    }
}