using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PSForeverLauncher.Models;

namespace PSForeverLauncher.Converters;

public sealed class FactionToColorConverter : IValueConverter
{
    private static readonly SolidColorBrush TRBrush = new(Color.FromRgb(0xE5, 0x39, 0x35));
    private static readonly SolidColorBrush NCBrush = new(Color.FromRgb(0x1E, 0x88, 0xE5));
    private static readonly SolidColorBrush VSBrush = new(Color.FromRgb(0x8E, 0x24, 0xAA));
    private static readonly SolidColorBrush UnknownBrush = new(Color.FromRgb(0x6B, 0x6B, 0x6B));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Faction faction ? faction switch
        {
            Faction.TR => TRBrush,
            Faction.NC => NCBrush,
            Faction.VS => VSBrush,
            _ => UnknownBrush
        } : UnknownBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
