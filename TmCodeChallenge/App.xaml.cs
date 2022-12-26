using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using TmCodeChallenge.Model.Services.VolumeStreams;

namespace TmCodeChallenge;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<MainWindow>();
                services.AddTransient<ITwitterVolumeService, TwitterVolumnService>();
                services.AddTransient<ILongRequester, LongRequester>();
            })
            .Build();
    }

    protected async override void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
        startupForm.Show();

        base.OnStartup(e);
    }

    protected async override void OnExit(ExitEventArgs e)
    {

        await AppHost!.StopAsync();
        base.OnExit(e);
    }
}
