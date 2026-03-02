using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PSForeverLauncher.Services;
using PSForeverLauncher.ViewModels;
using PSForeverLauncher.Views;

namespace PSForeverLauncher;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Services
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IServerQueryService, ServerQueryService>();
        services.AddSingleton<IGameDetectionService, GameDetectionService>();
        services.AddSingleton<IClientIniService, ClientIniService>();
        services.AddSingleton<IGameLaunchService, GameLaunchService>();

        // ViewModels
        services.AddSingleton<MainWindowViewModel>();

        // Views
        services.AddSingleton<MainWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
