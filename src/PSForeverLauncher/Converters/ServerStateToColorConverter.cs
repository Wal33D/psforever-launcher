using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PSForeverLauncher.Models;

namespace PSForeverLauncher.Converters;

public sealed class ServerStateToColorConverter : IValueConverter
{
    private static readonly SolidColorBrush OnlineBrush = new(Color.FromRgb(0x00, 0xE6, 0x76));
    private static readonly SolidColorBrush OfflineBrush = new(Color.FromRgb(0xE5, 0x39, 0x35));
    private static readonly SolidColorBrush UnknownBrush = new(Color.FromRgb(0xFF, 0xB7, 0x4D));
    private static readonly SolidColorBrush QueryingBrush = new(Color.FromRgb(0x00, 0xB4, 0xD8));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is ServerConnectionState state ? state switch
        {
            ServerConnectionState.Online => OnlineBrush,
            ServerConnectionState.Offline => OfflineBrush,
            ServerConnectionState.Querying => QueryingBrush,
            _ => UnknownBrush
        } : UnknownBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
