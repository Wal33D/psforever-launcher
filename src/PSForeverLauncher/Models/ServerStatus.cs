namespace PSForeverLauncher.Models;

public enum ServerConnectionState
{
    Unknown,
    Online,
    Offline,
    Querying
}

public sealed class ServerStatus
{
    public ServerConnectionState State { get; init; } = ServerConnectionState.Unknown;
    public int PlayerCount { get; init; }
    public List<PlayerInfo> Players { get; init; } = [];
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;
    public string? ErrorMessage { get; init; }

    public int TRCount => Players.Count(p => p.Faction == Faction.TR);
    public int NCCount => Players.Count(p => p.Faction == Faction.NC);
    public int VSCount => Players.Count(p => p.Faction == Faction.VS);

    public static ServerStatus CreateOffline(string? error = null) => new()
    {
        State = ServerConnectionState.Offline,
        ErrorMessage = error
    };

    public static ServerStatus CreateUnknown() => new()
    {
        State = ServerConnectionState.Unknown
    };
}
