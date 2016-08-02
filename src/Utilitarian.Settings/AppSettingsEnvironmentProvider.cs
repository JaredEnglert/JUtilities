using System.Configuration;

namespace Utilitarian.Settings
{
    public class AppSettingsEnvironmentProvider : IEnvironmentProvider
    {
        public string EnvironmentName => ConfigurationManager.AppSettings["EnvironmentProvider:EnvironmentName"];
    }
}