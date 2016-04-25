using System.Configuration;

namespace JUtilities.Settings
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
