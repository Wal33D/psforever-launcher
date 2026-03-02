using System.Text.Json.Serialization;

namespace PSForeverLauncher.Models;

public sealed class LauncherSettings
{
    public string? GamePath { get; set; }
    public string? ClientIniPath { get; set; }
    public List<ServerEntry> Servers { get; set; } = [];
    public string? SelectedServerId { get; set; }
    public string LaunchArgs { get; set; } = "/K:StagingTest /CC";
    public double WindowLeft { get; set; } = double.NaN;
    public double WindowTop { get; set; } = double.NaN;
    public double WindowWidth { get; set; } = 900;
    public double WindowHeight { get; set; } = 600;
    public bool IsMaximized { get; set; }
    public bool FirstRun { get; set; } = true;
    public int PollIntervalSeconds { get; set; } = 15;

    [JsonIgnore]
    public static ServerEntry DefaultServer => new()
    {
        Id = "psforever",
        Name = "PSForever",
        Host = "play.psforever.net",
        LoginPort = 51000,
        AdminPort = 51002,
        IsDefault = true
    };
}
