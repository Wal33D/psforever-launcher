using System.Windows;
using System.Windows.Controls;
using PSForeverLauncher.Models;

namespace PSForeverLauncher.Controls;

public partial class FactionBadge : UserControl
{
    public static readonly DependencyProperty FactionProperty = DependencyProperty.Register(
        nameof(Faction), typeof(Faction), typeof(FactionBadge),
        new PropertyMetadata(Faction.Unknown));

    public static readonly DependencyProperty CountProperty = DependencyProperty.Register(
        nameof(Count), typeof(int), typeof(FactionBadge),
        new PropertyMetadata(0));

    public Faction Faction
    {
        get => (Faction)GetValue(FactionProperty);
        set => SetValue(FactionProperty, value);
    }

    public int Count
    {
        get => (int)GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }

    public FactionBadge()
    {
        InitializeComponent();
    }
}
