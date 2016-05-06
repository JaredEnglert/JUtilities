using System;
using System.Collections.Generic;

namespace JUtilities.Status
{
    public interface IStatusCheckService
    {
        DateTime LastUpdatedUtc { get; }

        int PollIncrement { get; }

        Status GetStatus(Type statusCheckType);

        IEnumerable<Status> GetStatuses();
    }
}
