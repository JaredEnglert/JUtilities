using System.Configuration;

namespace Utilitarian.Settings
{
    public class AppSettingsEnvironmentProvider : IEnvironmentProvider
    {
        public string EnvironmentName
        {
            get
            {
                return ConfigurationManager.AppSettings["EnvironmentProvider:EnvironmentName"];
            }
        }
    }
}