namespace Utilities.Settings
{
    public interface IConnectionStringProvider
    {
        string Get(string key);

        void ForceUpdate();
    }
}
