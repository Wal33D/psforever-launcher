using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSForeverLauncher.Models;
using PSForeverLauncher.Services;

namespace PSForeverLauncher.ViewModels;

public partial class ServerListViewModel : ObservableObject
{
    private readonly IServerQueryService _queryService;
    private readonly ISettingsService _settingsService;
    private readonly Action<ServerItemViewModel> _onPlayRequested;

    [ObservableProperty] private ObservableCollection<ServerItemViewModel> _servers = [];
    [ObservableProperty] private ServerItemViewModel? _selectedServer;

    public ServerListViewModel(
        IServerQueryService queryService,
        ISettingsService settingsService,
        Action<ServerItemViewModel> onPlayRequested)
    {
        _queryService = queryService;
        _settingsService = settingsService;
        _onPlayRequested = onPlayRequested;

        _queryService.StatusUpdated += OnStatusUpdated;
    }

    public void LoadServers()
    {
        Servers.Clear();
        foreach (var entry in _settingsService.Settings.Servers)
        {
            var vm = new ServerItemViewModel(entry, _onPlayRequested);
            Servers.Add(vm);

            // Apply cached status if available
            var cached = _queryService.GetStatus(entry);
            vm.UpdateStatus(cached);
        }

        // Select the saved server or first one
        SelectedServer = Servers.FirstOrDefault(s =>
            s.Entry.Id == _settingsService.Settings.SelectedServerId) ?? Servers.FirstOrDefault();

        if (SelectedServer != null)
            SelectedServer.IsSelected = true;
    }

    public void StartPolling(CancellationToken ct)
    {
        _queryService.StartPolling(
            _settingsService.Settings.Servers, ct);
    }

    public void StopPolling() => _queryService.StopPolling();

    private void OnStatusUpdated(ServerEntry entry, ServerStatus status)
    {
        var vm = Servers.FirstOrDefault(s => s.Entry.Id == entry.Id);
        // Dispatch to UI thread
        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            vm?.UpdateStatus(status);
        });
    }

    [RelayCommand]
    private void SelectServer(ServerItemViewModel? server)
    {
        if (SelectedServer != null)
            SelectedServer.IsSelected = false;

        SelectedServer = server;

        if (server != null)
        {
            server.IsSelected = true;
            _settingsService.Settings.SelectedServerId = server.Entry.Id;
        }
    }
}
