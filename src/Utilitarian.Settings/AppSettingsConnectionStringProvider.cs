using System;
using System.Configuration;

namespace Utilitarian.Settings
{
    public class AppSettingsConnectionStringProvider : IConnectionStringProvider
    {
        private const string Prefix = "ConnectionString:";

        public string Get(string key)
        {
            var connectionString = ConfigurationManager.AppSettings[$"{Prefix}{key}"];

            if (string.IsNullOrWhiteSpace(connectionString)) throw new Exception($"Could not find connection string with key \"{Prefix}{key}\" in the config's appSettings");

            return connectionString;
        }

        public void ForceUpdate()
        {
        }
    }
}
