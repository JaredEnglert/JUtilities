using System.Threading;
using Utilitarian.Settings;

namespace Utilitarian.Status.Test.Unit.Mocks
{
    public class TimeoutStatusCheck : IStatusCheck
    {
        private readonly ISettingsProvider _settingsProvider;

        public TimeoutStatusCheck(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public bool IsActive()
        {
            Thread.Sleep((_settingsProvider.Get<int>("StatusCheck:TimeoutLimitInSeconds") * 1000) + 1000);

            return true;
        }
    }
}
