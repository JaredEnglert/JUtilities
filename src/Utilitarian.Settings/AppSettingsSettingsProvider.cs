using System.Configuration;
using Utilitarian.Extensions;

namespace Utilitarian.Settings
{
    public class AppSettingsSettingsProvider : SettingsProviderBase, ISettingsProvider
    {
        public bool Contains(string key)
        {
            return ConfigurationManager.AppSettings[key] != null;
        }

        public override bool TryGet<T>(string key, out T value)
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
