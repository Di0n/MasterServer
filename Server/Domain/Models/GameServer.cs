using Server.Domain.Models;
using System.Net.WebSockets;

public class GameServer : IEquatable<GameServer>
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    // Host and port decide uniqueness
    public string Host { get; set; } = "";
    public uint Port { get; set; }
    public string Name { get; set; } = "";
    public string AuthToken { get; set; } = "";
    public WebSocket? Connection { get; set; }

    public GameInfo? Info { get; set; }
    public DateTimeOffset LastHeartbeat { get; set; }

    public bool Equals(GameServer? other)
    {
        if (other is null) return false;
        return string.Equals(Host, other.Host, StringComparison.OrdinalIgnoreCase)
            && Port == other.Port;
    }

    public override bool Equals(object? obj) => Equals(obj as GameServer);

    public override int GetHashCode()
    {
        return HashCode.Combine(Host.ToLowerInvariant(), Port);
    }
}