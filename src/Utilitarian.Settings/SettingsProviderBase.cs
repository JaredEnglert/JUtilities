using System;

namespace Utilitarian.Settings
{
    public abstract class SettingsProviderBase
    {
        public object Get(string key)
        {
            return Get<object>(key);
        }

        public T Get<T>(string key)
        {
            T value;

            if (!TryGet(key, out value)) throw new Exception($"No value was found for key \"{key}\"");

            return value;
        }

        public abstract bool TryGet<T>(string key, out T value);

        public virtual void ForceUpdate()
        {
        }
    }
}
