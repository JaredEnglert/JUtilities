using System;
using System.Configuration;
using Utilitarian.Extensions;

namespace Utilitarian.Settings
{
    public class AppSettingsSettingsProvider : ISettingsProvider
    {
        public bool Contains(string key)
        {
            return ConfigurationManager.AppSettings[key] != null;
        }

        public void ForceUpdate()
        {
        }

        public string Get(string key)
        {
            return Get<string>(key);
        }

        public T Get<T>(string key)
        {
            T value;

            if (!TryGet(key, out value)) throw new Exception($"No value was found for key \"{key}\"");

            return value;
        }

        public bool TryGet<T>(string key, out T value)
        {
            var settings = ConfigurationManager.AppSettings[key];

            if (settings == null)
            {
                value = default(T);
                return false;
            }

            value = settings.ToType<T>();

            return true;
        }
    }
}
