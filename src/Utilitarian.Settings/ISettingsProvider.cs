 namespace Utilitarian.Settings
{
    public interface ISettingsProvider
    {
        T Get<T>(string key);

        object Get(string key);

        bool TryGet<T>(string key, out T value);

        bool Contains(string key);

        void ForceUpdate();
    }
}
