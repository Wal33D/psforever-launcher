namespace PSForeverLauncher.Services;

public interface IGameLaunchService
{
    event Action<string>? OutputReceived;
    event Action<int>? GameExited;
    bool IsRunning { get; }
    Task LaunchAsync(string gamePath, string launchArgs);
    void Kill();
}
