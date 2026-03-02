namespace PSForeverLauncher.Models;

public sealed class PlayerInfo
{
    public string Name { get; init; } = string.Empty;
    public Faction Faction { get; init; } = Faction.Unknown;
    public int CharId { get; init; }
}
