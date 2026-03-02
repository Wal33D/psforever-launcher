using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSForeverLauncher.Models;
using PSForeverLauncher.Services;

namespace PSForeverLauncher.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly IClientIniService _clientIniService;
    private readonly IGameLaunchService _gameLaunchService;
    private readonly IGameDetectionService _gameDetectionService;
    private CancellationTokenSource? _pollCts;

    [ObservableProperty] private ServerListViewModel _serverList;
    [ObservableProperty] private SettingsViewModel _settings;
    [ObservableProperty] private string _statusText = "Ready";
    [ObservableProperty] private string _versionText = "v2.0.0";
    [ObservableProperty] private string _gameDetectionText = "Checking...";
    [ObservableProperty] private bool _isGameRunning;
    [ObservableProperty] private string _activeView = "Servers";

    public MainWindowViewModel(
        ISettingsService settingsService,
        IServerQueryService queryService,
        IClientIniService clientIniService,
        IGameLaunchService gameLaunchService,
        IGameDetectionService gameDetectionService)
    {
        _settingsService = settingsService;
        _clientIniService = clientIniService;
        _gameLaunchService = gameLaunchService;
        _gameDetectionService = gameDetectionService;

        _serverList = new ServerListViewModel(queryService, settingsService, OnPlayRequested);
        _settings = new SettingsViewModel(settingsService, gameDetectionService);

        _gameLaunchService.GameExited += OnGameExited;
    }

    public async Task InitializeAsync()
    {
        await _settingsService.LoadAsync();
        Settings.Load();
        ServerList.LoadServers();

        GameDetectionText = Settings.IsGameDetected ? "Game: Detected" : "Game: Not Found";

        _pollCts = new CancellationTokenSource();
        ServerList.StartPolling(_pollCts.Token);
    }

    private async void OnPlayRequested(ServerItemViewModel server)
    {
        if (IsGameRunning)
        {
            StatusText = "Game is already running";
            return;
        }

        var gamePath = _settingsService.Settings.GamePath;
        if (string.IsNullOrEmpty(gamePath) || !_gameDetectionService.ValidateGamePath(gamePath))
        {
            StatusText = "Game not found — check Settings";
            ActiveView = "Settings";
            return;
        }

        try
        {
            StatusText = $"Connecting to {server.ServerName}...";

            // Write client.ini
            _clientIniService.WriteClientIni(gamePath, server.Entry.Host, server.Entry.LoginPort);

            // Launch game
            await _gameLaunchService.LaunchAsync(gamePath, _settingsService.Settings.LaunchArgs);

            IsGameRunning = true;
            StatusText = $"Playing on {server.ServerName}";
        }
        catch (Exception ex)
        {
            StatusText = $"Launch failed: {ex.Message}";
        }
    }

    private void OnGameExited(int exitCode)
    {
        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            IsGameRunning = false;
            StatusText = exitCode == 0 ? "Ready" : $"Game exited (code {exitCode})";
        });
    }

    [RelayCommand]
    private void ShowServers() => ActiveView = "Servers";

    [RelayCommand]
    private void ShowSettings() => ActiveView = "Settings";

    [RelayCommand]
    private void ShowPlayers() => ActiveView = "Players";

    public async Task ShutdownAsync()
    {
        _pollCts?.Cancel();
        ServerList.StopPolling();
        _gameLaunchService.Kill();
        await _settingsService.SaveAsync();
    }
}
