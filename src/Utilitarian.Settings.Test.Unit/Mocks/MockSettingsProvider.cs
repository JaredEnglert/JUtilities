using System.Collections.Generic;
using Utilitarian.Extensions;

namespace Utilitarian.Settings.Test.Unit.Mocks
{
    public class MockSettingsProvider : SettingsProviderBase, ISettingsProvider
    {
        public Dictionary<string, object> Settings { get; set; }

        public MockSettingsProvider()
        {
            Settings = new Dictionary<string, object>();
        }

        public MockSettingsProvider(Dictionary<string, object> settings)
        {
            Settings = settings;
        }

        public bool Contains(string key)
        {
            return Settings.ContainsKey(key);
        }

        public override bool TryGet<T>(string key, out T value)
        {
            var settings = Settings[key];

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
