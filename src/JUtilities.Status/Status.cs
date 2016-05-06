using System;

namespace JUtilities.Status
{
    public class Status
    {
        public Type StatusCheckType { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastUpdatedUtc { get; set; }

        public Exception Exception { get; set; }
    }
}
