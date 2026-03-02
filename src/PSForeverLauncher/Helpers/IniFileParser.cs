using System.IO;

namespace PSForeverLauncher.Helpers;

public static class IniFileParser
{
    public static Dictionary<string, Dictionary<string, string>> Parse(string filePath)
    {
        var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        var currentSection = string.Empty;

        if (!File.Exists(filePath)) return result;

        foreach (var rawLine in File.ReadLines(filePath))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith(';') || line.StartsWith('#'))
                continue;

            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                currentSection = line[1..^1].Trim();
                if (!result.ContainsKey(currentSection))
                    result[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                continue;
            }

            var eqIndex = line.IndexOf('=');
            if (eqIndex <= 0) continue;

            var key = line[..eqIndex].Trim();
            var value = line[(eqIndex + 1)..].Trim();

            if (!result.ContainsKey(currentSection))
                result[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            result[currentSection][key] = value;
        }

        return result;
    }
}
