using System.Configuration;

namespace Utilitarian.Settings
{
    public class AppSettingsApplicationProvider: IApplicationProvider
    {
        public string ApplicationName
        {
            get
            {
                return ConfigurationManager.AppSettings["ApplicationProvider:ApplicationName"];
            }
        }
    }
}
