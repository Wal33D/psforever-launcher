using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PSForeverLauncher.Services;

namespace PSForeverLauncher.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly IGameDetectionService _gameDetectionService;

    [ObservableProperty] private string _gamePath = string.Empty;
    [ObservableProperty] private bool _isGameDetected;
    [ObservableProperty] private string _launchArgs = string.Empty;
    [ObservableProperty] private string _gameStatus = "Not detected";

    public SettingsViewModel(ISettingsService settingsService, IGameDetectionService gameDetectionService)
    {
        _settingsService = settingsService;
        _gameDetectionService = gameDetectionService;
    }

    public void Load()
    {
        var settings = _settingsService.Settings;

        if (!string.IsNullOrEmpty(settings.GamePath) && _gameDetectionService.ValidateGamePath(settings.GamePath))
        {
            GamePath = settings.GamePath;
            IsGameDetected = true;
            GameStatus = "Detected";
        }
        else
        {
            // Try auto-detection
            var detected = _gameDetectionService.DetectGamePath();
            if (detected != null)
            {
                GamePath = detected;
                IsGameDetected = true;
                GameStatus = "Auto-detected";
                settings.GamePath = detected;
            }
            else
            {
                GamePath = string.Empty;
                IsGameDetected = false;
                GameStatus = "Not detected";
            }
        }

        LaunchArgs = settings.LaunchArgs;
    }

    [RelayCommand]
    private void BrowseGamePath()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Locate planetside.exe",
            Filter = "PlanetSide|planetside.exe|All Files|*.*",
            CheckFileExists = true
        };

        if (dialog.ShowDialog() == true)
        {
            var dir = System.IO.Path.GetDirectoryName(dialog.FileName);
            if (dir != null && _gameDetectionService.ValidateGamePath(dir))
            {
                GamePath = dir;
                IsGameDetected = true;
                GameStatus = "Manually set";
                _settingsService.Settings.GamePath = dir;
            }
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        _settingsService.Settings.LaunchArgs = LaunchArgs;
        await _settingsService.SaveAsync();
    }
}
