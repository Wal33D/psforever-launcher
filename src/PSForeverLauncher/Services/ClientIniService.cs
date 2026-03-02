using System.IO;

namespace PSForeverLauncher.Services;

public sealed class ClientIniService : IClientIniService
{
    public void WriteClientIni(string gamePath, string host, int port)
    {
        var iniPath = Path.Combine(gamePath, "client.ini");
        var content = $"[network]\r\nlogin0={host}:{port}\r\n";
        File.WriteAllText(iniPath, content);
    }

    public (string? Host, int Port)? ReadClientIni(string gamePath)
    {
        var iniPath = Path.Combine(gamePath, "client.ini");
        if (!File.Exists(iniPath)) return null;

        foreach (var line in File.ReadLines(iniPath))
        {
            var trimmed = line.Trim();
            if (!trimmed.StartsWith("login0=", StringComparison.OrdinalIgnoreCase))
                continue;

            var value = trimmed["login0=".Length..];
            var colonIndex = value.LastIndexOf(':');
            if (colonIndex <= 0) continue;

            var host = value[..colonIndex];
            if (int.TryParse(value[(colonIndex + 1)..], out var port))
                return (host, port);
        }

        return null;
    }
}
