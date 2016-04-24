using System.Configuration;

namespace Utilities.Settings
{
    public class AppSettingsConnectionStringProvider : IConnectionStringProvider
    {
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[string.Format("ConnectionString:{0}", key)];
        }

        public void ForceUpdate()
        {
        }
    }
}
