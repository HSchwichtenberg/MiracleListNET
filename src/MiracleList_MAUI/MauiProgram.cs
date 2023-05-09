using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MiracleList;
using MiracleList_MAUI.Services;
using MiracleList_MAUI.ViewModels;
using MiracleList_MAUI.Views;
using System.Reflection;

namespace MiracleList_MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            ReadConfig(builder);
            RegisterServices(builder.Services);
#if DEBUG
            builder.Logging.AddDebug();
#endif


            return builder.Build();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<App>();
            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
            services.AddSingleton<ISecureStorage>(SecureStorage.Default);
            services.AddSingleton<IPreferences>(Preferences.Default);
            services.AddSingleton<IBrowser>(Browser.Default);
            services.AddSingleton<IAppState, AppState>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton(new HttpClient());
            services.AddSingleton<IMiracleListProxy, MiracleListProxy>();

            services.AddScoped<AppShell>();
            services.AddScoped<AppShellViewModel>();

            services.AddScoped<MainPage>();
            services.AddScoped<CategoriesPage>();
            services.AddScoped<CategoriesPageViewModel>();

            services.AddScoped<TasksPageViewModel>();
            services.AddScoped<TasksPage>();

            services.AddScoped<TaskDetailsPageViewModel>();
            services.AddScoped<TaskDetailsPage>();

            // View und ViewModel gemeinsam über das Community Toolkit registrieren
            services.AddScoped<LoginPage, LoginPageViewModel>();
        }

        private static void ReadConfig(MauiAppBuilder builder)
        {
            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("MiracleList_MAUI.appsettings.json");
            var config = new ConfigurationBuilder()
               .AddJsonStream(stream)
               .Build();
            builder.Configuration.AddConfiguration(config);
        }
    }
}