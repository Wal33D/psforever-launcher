using System.Windows;
using System.Windows.Controls;
using PSForeverLauncher.Models;

namespace PSForeverLauncher.Controls;

public partial class StatusIndicator : UserControl
{
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        nameof(State), typeof(ServerConnectionState), typeof(StatusIndicator),
        new PropertyMetadata(ServerConnectionState.Unknown));

    public ServerConnectionState State
    {
        get => (ServerConnectionState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public StatusIndicator()
    {
        InitializeComponent();
    }
}
