using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JUtilities.Settings;

namespace JUtilities.Status
{
    public class StatusCheckService : IStatusCheckService
    {
        public DateTime LastUpdatedUtc { get; private set; }

        public int PollIncrement
        {
            get
            {
                return _settingsProvider.Get<int>("StatusCheck:PollIncrementInSeconds") * 1000;
            }
        }

        private readonly ConcurrentDictionary<Type, Status> _statuses = new ConcurrentDictionary<Type, Status>();

        private readonly ISettingsProvider _settingsProvider;

        private readonly IEnumerable<IStatusCheck> _statusChecks;

        public StatusCheckService(ISettingsProvider settingsProvider, IEnumerable<IStatusCheck> statusChecks)
        {
            _settingsProvider = settingsProvider;
            _statusChecks = statusChecks;

            Update();

            Task.Delay(PollIncrement).ContinueWith(a => Task.Run(() => Poll()));
        }

        public Status GetStatus(Type statusCheckType)
        {
            return _statuses.ContainsKey(statusCheckType)
                ? _statuses[statusCheckType]
                : null;
        }

        public IEnumerable<Status> GetStatuses()
        {
            return _statuses.Values;
        }

        private void Update()
        {
            _statusChecks.AsParallel().ForAll(statusCheck => 
            {
                var status = new Status
                {
                    StatusCheckType = statusCheck.GetType(),
                    IsActive = true
                };

                try
                {
                    var timeout = _settingsProvider.Get<int>("StatusCheck:TimeoutLimitInMilliseconds");

                    if (!Task.Run(() => status.IsActive = statusCheck.IsActive()).Wait(timeout))
                    {
                        status.IsActive = false;
                        status.Exception = new TimeoutException(string.Format("Testing the status took longer than {0} seconds.", timeout / 1000));
                    }
                }
                catch (Exception exception)
                {
                    status.IsActive = false;
                    status.Exception = exception;
                }

                status.LastUpdatedUtc = DateTime.UtcNow;

                _statuses.AddOrUpdate(status.StatusCheckType, status, (type, oldStatus) => status);
            });

            LastUpdatedUtc = DateTime.UtcNow;
        }

        private void Poll()
        {
            try
            {
                Update();
            }
            finally
            {
                Task.Delay(PollIncrement).ContinueWith(a => Task.Run(() => Poll()));
            }
        }
    }
}
