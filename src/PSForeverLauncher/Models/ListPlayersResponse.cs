using System.Text.Json.Serialization;

namespace PSForeverLauncher.Models;

public sealed class ListPlayersResponse
{
    [JsonPropertyName("count")]
    public int Count { get; init; }

    [JsonPropertyName("players")]
    public List<ListPlayersEntry> Players { get; init; } = [];
}

public sealed class ListPlayersEntry
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("faction")]
    public string Faction { get; init; } = string.Empty;

    [JsonPropertyName("char_id")]
    public int CharId { get; init; }
}
