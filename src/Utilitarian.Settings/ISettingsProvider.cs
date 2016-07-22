 namespace Utilitarian.Settings
{
    public interface ISettingsProvider
    {
        T Get<T>(string key);

        string Get(string key);

        bool TryGet<T>(string key, out T value);

        bool Contains(string key);

        void ForceUpdate();
    }
}
