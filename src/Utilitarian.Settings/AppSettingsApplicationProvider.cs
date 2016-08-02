using System.Configuration;

namespace Utilitarian.Settings
{
    public class AppSettingsApplicationProvider: IApplicationProvider
    {
        public string ApplicationName => ConfigurationManager.AppSettings["ApplicationProvider:ApplicationName"];
    }
}
