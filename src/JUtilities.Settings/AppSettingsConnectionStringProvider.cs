using System;
using System.Configuration;

namespace JUtilities.Settings
{
    public class AppSettingsConnectionStringProvider : IConnectionStringProvider
    {
        private const string Prefix = "ConnectionString:";

        public string Get(string key)
        {
            var connectionString = ConfigurationManager.AppSettings[string.Format("{0}{1}", Prefix, key)];

            if (string.IsNullOrWhiteSpace(connectionString)) throw new Exception(string.Format("Could not find connection string with key \"{0}{1}\" in the config's appSettings", Prefix, key));

            return connectionString;
        }

        public void ForceUpdate()
        {
        }
    }
}
