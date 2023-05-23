using Microsoft.Extensions.Configuration;
using MiracleList;
using MiracleList_WinUI;
using MiracleList_WinUI.Dialogs;
using MiracleList_WinUI.ViewModels;
using MiracleList_WinUI.Views;
using MvvmGen.Events;
using System.Reflection;

namespace MiracleList_Uno
{
    public class App : Application
    {
        public static Window? _window;
        public App()
        {

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; }

        private static void ConfigureServices(ServiceCollection services)
        {
            var builder = new ConfigurationBuilder();
            //   .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            //   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //   .AddUserSecrets(Assembly.GetExecutingAssembly(), true);
            var configuration = builder.Build();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IAppState, AppState>();
            services.AddSingleton(new HttpClient());
            services.AddSingleton<IEventAggregator, EventAggregator>();
            services.AddTransient<IDialogService>(sp =>
            {
                var dialogService = new DialogService();
                return dialogService;
            });
            services.AddTransient<AboutView>();
            services.AddTransient<TaskManagementView>();
            services.AddTransient<TaskManagementViewModel>();
            services.AddTransient<LoginView>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<ITaskItemViewModelFactory, TaskItemViewModelFactory>();
            services.AddTransient<ITaskDetailViewModelFactory, TaskDetailViewModelFactory>();
            services.AddTransient<ICategoryItemViewModelFactory, CategoryItemViewModelFactory>();
            services.AddScoped<IMiracleListProxy>(sp =>
            {
                var appState = sp.GetService<IAppState>();
                return new MiracleListProxy(sp.GetService<HttpClient>())
                {
                    BackendUrl = appState?.BackendURL
                };
            });
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
		_window = new Window();
#else
            _window = Microsoft.UI.Xaml.Window.Current;
#endif

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (_window.Content is not Frame rootFrame)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // Place the frame in the current Window
                _window.Content = rootFrame;

                rootFrame.NavigationFailed += OnNavigationFailed;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), args.Arguments);
            }

            // Ensure the current window is active
            _window.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
        }
    }
}