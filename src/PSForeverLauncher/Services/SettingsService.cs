using System.IO;
using System.Text.Json;
using PSForeverLauncher.Models;

namespace PSForeverLauncher.Services;

public sealed class SettingsService : ISettingsService
{
    private static readonly string SettingsDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "PSForeverLauncher");

    private static readonly string SettingsPath = Path.Combine(SettingsDir, "settings.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public LauncherSettings Settings { get; private set; } = new();

    public async Task LoadAsync()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = await File.ReadAllTextAsync(SettingsPath);
                Settings = JsonSerializer.Deserialize<LauncherSettings>(json, JsonOptions) ?? new();
            }
        }
        catch
        {
            Settings = new LauncherSettings();
        }

        EnsureDefaultServer();
    }

    public async Task SaveAsync()
    {
        try
        {
            Directory.CreateDirectory(SettingsDir);
            var json = JsonSerializer.Serialize(Settings, JsonOptions);
            await File.WriteAllTextAsync(SettingsPath, json);
        }
        catch
        {
            // Silently fail — settings are non-critical
        }
    }

    private void EnsureDefaultServer()
    {
        if (Settings.Servers.All(s => s.Id != "psforever"))
        {
            Settings.Servers.Insert(0, LauncherSettings.DefaultServer);
        }

        Settings.SelectedServerId ??= "psforever";
    }
}
