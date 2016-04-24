using System;
using System.Configuration;
using Utilities.Extensions;

namespace Utilities.Settings
{
    public class AppSettingsProvider : ISettingsProvider
    {
        public string ApplicationName
        {
            get
            {
                return ConfigurationManager.AppSettings["SettingsProvider:ApplicationName"];
            }
        }

        public string EnvironmentName
        {
            get
            {
                return ConfigurationManager.AppSettings["SettingsProvider:EnvironmentName"];
            }
        }

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

            if (!TryGet(key, out value)) throw new Exception(string.Format("No value was found for key \"{0}\"", key));

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
