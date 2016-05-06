using System.Threading;
using JUtilities.Settings;

namespace JUtilities.Status.Test.Unit.Mocks
{
    public class TimeoutStatusCheck : IStatusCheck
    {
        public bool IsActive()
        {
            var settingsProvider = new AppSettingsSettingsProvider();

            Thread.Sleep(settingsProvider.Get<int>("StatusCheck:TimeoutLimitInMilliseconds") + 1000);

            return true;
        }
    }
}
