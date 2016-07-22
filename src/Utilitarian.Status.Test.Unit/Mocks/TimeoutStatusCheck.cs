using System.Threading;
using Utilitarian.Settings;

namespace Utilitarian.Status.Test.Unit.Mocks
{
    public class TimeoutStatusCheck : IStatusCheck
    {
        public bool IsActive()
        {
            var settingsProvider = new AppSettingsSettingsProvider();

            Thread.Sleep((settingsProvider.Get<int>("StatusCheck:TimeoutLimitInSeconds") * 1000) + 1000);

            return true;
        }
    }
}
