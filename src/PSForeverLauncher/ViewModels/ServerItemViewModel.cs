using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSForeverLauncher.Models;

namespace PSForeverLauncher.ViewModels;

public partial class ServerItemViewModel : ObservableObject
{
    private readonly ServerEntry _entry;
    private readonly Action<ServerItemViewModel> _onPlay;

    [ObservableProperty] private string _serverName;
    [ObservableProperty] private string _address;
    [ObservableProperty] private ServerConnectionState _connectionState = ServerConnectionState.Unknown;
    [ObservableProperty] private int _playerCount;
    [ObservableProperty] private int _tRCount;
    [ObservableProperty] private int _nCCount;
    [ObservableProperty] private int _vSCount;
    [ObservableProperty] private bool _isSelected;

    public ServerEntry Entry => _entry;

    public ServerItemViewModel(ServerEntry entry, Action<ServerItemViewModel> onPlay)
    {
        _entry = entry;
        _onPlay = onPlay;
        _serverName = entry.Name;
        _address = entry.DisplayAddress;
    }

    public void UpdateStatus(ServerStatus status)
    {
        ConnectionState = status.State;
        PlayerCount = status.PlayerCount;
        TRCount = status.TRCount;
        NCCount = status.NCCount;
        VSCount = status.VSCount;
    }

    [RelayCommand]
    private void Play() => _onPlay(this);
}
