using System;
using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using MiracleList;
using MiracleList_WinUI.ViewModels;
using MiracleList_WinUI.Views;
using MvvmGen.Events;


namespace MiracleList_WinUI;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();

        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        var builder = new ConfigurationBuilder()
           .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddUserSecrets(Assembly.GetExecutingAssembly(), true);
        var configuration = builder.Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<IAppState, AppState>();
        services.AddSingleton(new HttpClient());
        services.AddSingleton<IEventAggregator, EventAggregator>();
        services.AddSingleton<MainWindow>();
        services.AddTransient<AboutView>();
        services.AddTransient<TaskManagementView>();
        services.AddTransient<LoginView>();
        services.AddTransient<LoginViewModel>();
        services.AddScoped<IMiracleListProxy>(sp =>
        {
            var appState = sp.GetService<IAppState>();
            return new MiracleListProxy(sp.GetService<HttpClient>())
            {
                BackendUrl = appState?.BackendURL
            };
        });
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        m_window = ServiceProvider.GetService<MainWindow>();
        m_window?.Activate();
    }

    private Window? m_window;

    public ServiceProvider ServiceProvider { get; private set; }
}
