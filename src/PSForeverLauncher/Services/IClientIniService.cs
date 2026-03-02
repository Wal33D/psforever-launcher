namespace PSForeverLauncher.Services;

public interface IClientIniService
{
    void WriteClientIni(string gamePath, string host, int port);
    (string? Host, int Port)? ReadClientIni(string gamePath);
}
