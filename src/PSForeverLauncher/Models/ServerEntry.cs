namespace PSForeverLauncher.Models;

public sealed class ServerEntry
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];
    public string Name { get; init; } = string.Empty;
    public string Host { get; init; } = string.Empty;
    public int LoginPort { get; init; } = 51000;
    public int AdminPort { get; init; } = 51002;
    public bool IsDefault { get; init; }
    public bool IsCustom { get; init; }

    public string DisplayAddress => $"{Host}:{LoginPort}";
    public string AdminAddress => $"{Host}:{AdminPort}";
}
