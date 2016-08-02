using System;
using System.Collections.Concurrent;
using Utilitarian.Extensions;

namespace Utilitarian.Caching.Test.Unit.Mocks
{
    public class MockCacheService : CacheServiceBase, ICacheService
    {
        private readonly Guid _id;

        private readonly ConcurrentDictionary<string, MockCacheItem> _cache;

        public MockCacheService()
        {
            _id = Guid.NewGuid();
            _cache = new ConcurrentDictionary<string, MockCacheItem>();
        }

        public void Set(string key, object value)
        {
            _cache.TryAdd(TransformKey(key), new MockCacheItem(value));
        }

        public void SetExpiring(string key, object value, TimeSpan expiration, bool rollExpirationOnAccess = false)
        {
            _cache.TryAdd(TransformKey(key), new MockCacheItem(value, rollExpirationOnAccess: rollExpirationOnAccess, initialDuration: expiration));
        }

        public void SetSingleUse(string key, object value)
        {
            _cache.TryAdd(TransformKey(key), new MockCacheItem(value, true, false));
        }

        public void SetSingleUseExpiring(string key, object value, TimeSpan expiration, bool rollExpirationOnAccess = false)
        {
            _cache.TryAdd(TransformKey(key), new MockCacheItem(value, true, rollExpirationOnAccess, expiration));
        }

        public override bool TryGet<T>(string key, out T value)
        {
            MockCacheItem cacheItem;
            if (!_cache.TryGetValue(TransformKey(key), out cacheItem) || DateTime.Now > cacheItem.Expiration)
            {
                value = default(T);

                return false;
            }

            if (cacheItem.IsSingleUse) _cache.TryRemove(key, out cacheItem);
            else if (cacheItem.RollExpirationOnAccess && cacheItem.InitialDuration.HasValue)
            {
                cacheItem.UpdateExpiration();
                _cache.TryUpdate(key, cacheItem, cacheItem);
            }

            value = cacheItem.Object.ToType<T>();
            
            return true;
        }

        public void Clear()
        {
            _cache.Clear();
        }

        private string TransformKey(string key)
        {
            return $"{_id}:{key}";
        }

        private class MockCacheItem : CacheItem
        {
            public DateTime Expiration { get; private set; }

            private static readonly TimeSpan DefaultExpiration = new TimeSpan(0, 5, 0);

            public MockCacheItem(object @object, bool isSingleUse = false, bool rollExpirationOnAccess = true, TimeSpan? initialDuration = default(TimeSpan?)) 
                : base(@object, isSingleUse, rollExpirationOnAccess, initialDuration ?? DefaultExpiration)
            {
                Expiration = DateTime.Now.Add(initialDuration ?? DefaultExpiration);
            }

            internal void UpdateExpiration()
            {
                Expiration = DateTime.Now.Add(InitialDuration ?? DefaultExpiration);
            }
        }
    }
}
