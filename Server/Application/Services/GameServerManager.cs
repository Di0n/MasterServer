using Server.Api.DTOs;
using Server.Application.Services;
using Server.Domain.Models;
using Server.Utils;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Security.Cryptography;

public class GameServerManager : IGameServerManager
{
    private readonly ConcurrentSet<GameServer> servers = new();
    private readonly IConfiguration configuration;
    private readonly double heartbeatInterval;
    
    public GameServerManager(IConfiguration configuration)
    {
        this.configuration = configuration;
        heartbeatInterval = configuration.GetValue<double>("MasterServer:HeartbeatInterval");
    }

    public GameServer RegisterServer(string name, string host, uint port, WebSocket connection)
    {
        GameServer server = new GameServer
        {
            Name = name,
            Host = host,
            Port = port,
            LastHeartbeat = DateTimeOffset.UtcNow,
            AuthToken = GenerateAuthToken(),
            Connection = connection
        };

        servers.TryAdd(server);

        return server;
    }

    public void UnregisterServer(string serverId)
    {
        var server = servers.Single(s => s.Id == serverId);
        UnregisterServer(server);
    }

    public void UnregisterServer(WebSocket socket)
    {
        var server = servers.Single(s => s.Connection == socket);
        UnregisterServer(server);
    }

    public void UnregisterServer(GameServer server) => servers.TryRemove(server);

    public void UnregisterInactiveServers()
    {
        var threshold = DateTime.UtcNow.AddSeconds(-heartbeatInterval); // Only retrieve servers with heartbeat in the last x seconds
        var inactiveServers = servers
            .Where(s => s.LastHeartbeat < threshold);

        foreach (var server in inactiveServers)
            UnregisterServer(server);
    }

    public GameServer? GetServer(string serverId)
    {
        return servers.Single(s => s.Id == serverId); //servers.TryGetValue(serverId, out var server) ? server : null;
    }

    public GameServer? GetServer(WebSocket socket)
    {
        return servers.Single(s => s.Connection == socket);
    }
    public void UpdateServerStatus(WebSocket socket, GameInfo update)
    {
        GameServer? server = GetServer(socket);
        server?.Info = update;
        server?.LastHeartbeat = DateTimeOffset.UtcNow;
    }

    public IEnumerable<GameServerDto> GetActiveServers()
    {
        var threshold = DateTime.UtcNow.AddSeconds(-heartbeatInterval); // Only retrieve servers with heartbeat in the last x seconds
        return servers
            .Where(s => s.LastHeartbeat > threshold)
            .Select(s => GameServerDto.ToDto(s)); // Don't expose AuthToken
    }

    private string GenerateAuthToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    public bool ValidateAuthToken(string serverId, string token)
    {
        throw new NotImplementedException();
        //return servers.TryGetValue(serverId, out var server) && 
        //       server.AuthToken == token;
    }

    



    public void UpdateServerStatus(string serverId, GameInfo update)
    {
        throw new NotImplementedException();
    }
}