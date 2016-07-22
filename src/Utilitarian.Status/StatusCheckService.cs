using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utilitarian.Settings;

namespace Utilitarian.Status
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

        public int TimeoutLimit
        {
            get
            {
                return _settingsProvider.Get<int>("StatusCheck:TimeoutLimitInSeconds") * 1000;
            }
        }

        private readonly ConcurrentDictionary<Type, Status> _statuses = new ConcurrentDictionary<Type, Status>();

        private readonly ISettingsProvider _settingsProvider;

        private readonly IEnumerable<IStatusCheck> _statusChecks;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

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
                    if (!Task.Run(() => status.IsActive = statusCheck.IsActive()).Wait(TimeoutLimit))
                    {
                        status.IsActive = false;
                        status.Exception = new TimeoutException(string.Format("Testing the status took longer than {0} seconds.", TimeoutLimit / 1000));
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

        public void ForceUpdate()
        {
            _cancellationTokenSource.Cancel();
            Poll();
        }

        private void Poll()
        {
            try
            {
                Update();
            }
            finally
            {
                Task.Delay(PollIncrement, _cancellationTokenSource.Token).ContinueWith(a => Task.Run(() => Poll()), _cancellationTokenSource.Token);
            }
        }
    }
}
