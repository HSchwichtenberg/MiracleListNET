namespace MiracleList_MAUI.Services
{
    public interface INavigationService
    {
     
        public Task GoToAsync(string url);
        public Task GoToAsync(string url, bool animate);
        public Task GoToAsync(string url, IDictionary<string, object> parameters);
        public Task GoToAsync(string url, bool animate, IDictionary<string, object> parameters);
    }
}