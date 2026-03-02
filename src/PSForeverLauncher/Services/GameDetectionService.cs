using System.IO;
using Microsoft.Win32;

namespace PSForeverLauncher.Services;

public sealed class GameDetectionService : IGameDetectionService
{
    private static readonly string[] KnownPaths =
    [
        @"C:\Program Files (x86)\Sony Online Entertainment\Installed Games\PlanetSide",
        @"C:\Program Files\Sony Online Entertainment\Installed Games\PlanetSide",
        @"C:\Games\PlanetSide",
        @"C:\PlanetSide",
        @"D:\Games\PlanetSide",
        @"D:\PlanetSide",
    ];

    private static readonly string[] RegistryKeys =
    [
        @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\PlanetSide",
        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\PlanetSide",
        @"SOFTWARE\WOW6432Node\Sony Online Entertainment\Installed Games\PlanetSide",
        @"SOFTWARE\Sony Online Entertainment\Installed Games\PlanetSide",
    ];

    public string? DetectGamePath()
    {
        // Try registry first
        foreach (var keyPath in RegistryKeys)
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(keyPath);
                var installPath = key?.GetValue("InstallLocation") as string
                    ?? key?.GetValue("InstallPath") as string;
                if (installPath != null && ValidateGamePath(installPath))
                    return installPath;
            }
            catch
            {
                // Registry access may fail — continue
            }
        }

        // Try known filesystem paths
        foreach (var path in KnownPaths)
        {
            if (ValidateGamePath(path))
                return path;
        }

        return null;
    }

    public bool ValidateGamePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        return File.Exists(Path.Combine(path, "planetside.exe"));
    }
}
