namespace MiracleList_MAUI.Services
{
    public class NavigationService : INavigationService
    {
        public Task GoToAsync(string url)
        {
            return Shell.Current.GoToAsync(url);
        }

        public Task GoToAsync(string url, bool animate)
        {
            return Shell.Current.GoToAsync(url, animate);
        }

        public Task GoToAsync(string url, IDictionary<string, object> parameters)
        {
            return Shell.Current.GoToAsync(url, parameters);
        }

        public Task GoToAsync(string url, bool animate, IDictionary<string, object> parameters)
        {
            return Shell.Current.GoToAsync(url, animate, parameters);
        }
    }
}
