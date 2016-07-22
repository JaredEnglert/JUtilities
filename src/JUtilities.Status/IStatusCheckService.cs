using System;
using System.Collections.Generic;

namespace JUtilities.Status
{
    public interface IStatusCheckService
    {
        DateTime LastUpdatedUtc { get; }

        int PollIncrement { get; }

        int TimeoutLimit { get; }

        Status GetStatus(Type statusCheckType);

        IEnumerable<Status> GetStatuses();

        void ForceUpdate();
    }
}
