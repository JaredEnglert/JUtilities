 namespace JUtilities.Settings
{
    public interface ISettingsProvider
    {
        string ApplicationName { get; }

        string EnvironmentName { get; }

        T Get<T>(string key);

        string Get(string key);

        bool TryGet<T>(string key, out T value);

        bool Contains(string key);

        void ForceUpdate();
    }
}
