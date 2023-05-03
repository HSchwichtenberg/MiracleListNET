using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MiracleList;
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
            services.AddSingleton<IAppState, AppState>();
            services.AddSingleton(new HttpClient());
            services.AddScoped<IMiracleListProxy, MiracleListProxy>();
            services.AddScoped<MainPage>();
            services.AddScoped<CategoriesPage>();
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