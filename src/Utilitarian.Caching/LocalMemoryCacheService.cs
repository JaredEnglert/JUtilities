using System;
using System.Collections.Specialized;
using System.Runtime.Caching;
using Utilitarian.Extensions;

namespace Utilitarian.Caching
{
    public class LocalMemoryCacheService : CacheServiceBase, ICacheService, IDisposable
    {
        private readonly bool _useDefault = true;

        private readonly string _name;

        private readonly NameValueCollection _config;

        private readonly TimeSpan _absoluteExpiration = new TimeSpan(24, 0, 0);

        private MemoryCache _memoryCache;
        
        public LocalMemoryCacheService()
        {
            _memoryCache = MemoryCache.Default;
        }

        public LocalMemoryCacheService(string name, NameValueCollection config = null)
        {
            _name = name;
            _config = config;
            _useDefault = false;

            _memoryCache = new MemoryCache(name, config);
        }

        public void Set(string key, object value)
        {
            _memoryCache.Add(key, new CacheItem(value, initialDuration: _absoluteExpiration), DateTime.Now.Add(_absoluteExpiration));
        }

        public void SetExpiring(string key, object value, TimeSpan expiration, bool rollExpirationOnAccess = false)
        {
            _memoryCache.Add(key, new CacheItem(value, rollExpirationOnAccess: rollExpirationOnAccess, initialDuration: expiration), DateTime.Now.Add(expiration));
        }

        public void SetSingleUse(string key, object value)
        {
            _memoryCache.Add(key, new CacheItem(value, true), DateTime.Now.Add(new TimeSpan(24, 0, 0)));
        }

        public void SetSingleUseExpiring(string key, object value, TimeSpan expiration, bool rollExpirationOnAccess = false)
        {
            _memoryCache.Add(key, new CacheItem(value, true, rollExpirationOnAccess), DateTime.Now.Add(expiration));
        }

        public override bool TryGet<T>(string key, out T value)
        {
            var cacheItem = _memoryCache.Get(key);

            if (cacheItem == null)
            {
                value = default(T);

                return false;
            }

            var castCacheItem = (CacheItem)cacheItem;

            if (castCacheItem.IsSingleUse) _memoryCache.Remove(key);
            else if (castCacheItem.RollExpirationOnAccess && castCacheItem.InitialDuration.HasValue) _memoryCache.Set(key, cacheItem, DateTime.Now.Add(castCacheItem.InitialDuration.Value));

            value = castCacheItem.Object.ToType<T>();

            return true;
        }

        public void Clear()
        {
            _memoryCache.Dispose();
            _memoryCache = _useDefault ? MemoryCache.Default : new MemoryCache(_name, _config);
        }
        public void Dispose()
        {
            _memoryCache?.Dispose();
        }
    }
}
