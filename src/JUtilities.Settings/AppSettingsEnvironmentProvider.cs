using System.Configuration;

namespace JUtilities.Settings
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