namespace Utilitarian.Settings
{
    public interface IConnectionStringProvider
    {
        string Get(string key);

        void ForceUpdate();
    }
}
