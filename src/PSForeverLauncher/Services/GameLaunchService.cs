using System.Diagnostics;

namespace PSForeverLauncher.Services;

public sealed class GameLaunchService : IGameLaunchService
{
    private Process? _process;

    public event Action<string>? OutputReceived;
    public event Action<int>? GameExited;
    public bool IsRunning => _process is { HasExited: false };

    public Task LaunchAsync(string gamePath, string launchArgs)
    {
        if (IsRunning)
            throw new InvalidOperationException("Game is already running");

        var exePath = System.IO.Path.Combine(gamePath, "planetside.exe");

        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = launchArgs,
                WorkingDirectory = gamePath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = false
            },
            EnableRaisingEvents = true
        };

        _process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is not null) OutputReceived?.Invoke(e.Data);
        };

        _process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is not null) OutputReceived?.Invoke($"[ERROR] {e.Data}");
        };

        _process.Exited += (_, _) =>
        {
            var exitCode = _process?.ExitCode ?? -1;
            GameExited?.Invoke(exitCode);
            _process?.Dispose();
            _process = null;
        };

        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        return Task.CompletedTask;
    }

    public void Kill()
    {
        try
        {
            if (IsRunning)
            {
                _process?.Kill(entireProcessTree: true);
            }
        }
        catch
        {
            // Process may have already exited
        }
    }
}
