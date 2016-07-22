using System;

namespace Utilitarian.Caching
{
    public class CacheItem
    {
        public CacheItem(object @object, bool isSingleUse = false, bool rollExpirationOnAccess = true, TimeSpan? initialDuration = null)
        {
            Object = @object;
            IsSingleUse = isSingleUse;
            RollExpirationOnAccess = rollExpirationOnAccess;
            InitialDuration = initialDuration;
        }

        public object Object { get; set; }

        public bool IsSingleUse { get; set; }

        public bool RollExpirationOnAccess { get; set; }

        public TimeSpan? InitialDuration { get; set; }
    }
}
