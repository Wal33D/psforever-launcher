using PSForeverLauncher.Models;

namespace PSForeverLauncher.Services;

public interface ISettingsService
{
    LauncherSettings Settings { get; }
    Task LoadAsync();
    Task SaveAsync();
}
