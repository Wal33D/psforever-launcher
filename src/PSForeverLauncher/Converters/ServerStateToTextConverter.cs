using System.Globalization;
using System.Windows.Data;
using PSForeverLauncher.Models;

namespace PSForeverLauncher.Converters;

public sealed class ServerStateToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is ServerConnectionState state ? state switch
        {
            ServerConnectionState.Online => "ONLINE",
            ServerConnectionState.Offline => "OFFLINE",
            ServerConnectionState.Querying => "CHECKING...",
            _ => "UNKNOWN"
        } : "UNKNOWN";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
