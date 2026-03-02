namespace PSForeverLauncher.Services;

public interface IGameDetectionService
{
    string? DetectGamePath();
    bool ValidateGamePath(string path);
}
