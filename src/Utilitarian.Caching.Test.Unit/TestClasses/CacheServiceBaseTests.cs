using System;
using System.Collections.Concurrent;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilitarian.Extensions;

namespace Utilitarian.Caching.Test.Unit.TestClasses
{
    [TestClass]
    public class CacheServiceBaseTests
    {
        private const string Key = "TestKey";

        private const string Value = "TestValue";

        [TestMethod]
        public void Get_Cached_ShouldReturnValue()
        {
            var cacheService = new MockCacheService();

            cacheService.Set(Key, Value);

            cacheService.Get<string>(Key).ShouldBeEquivalentTo(Value);
        }

        [TestMethod]
        public void Get_NotCached_ShouldThrowException()
        {
            var cacheService = new MockCacheService();

            Action action = () => cacheService.Get<string>(Key);

            action.ShouldThrow<Exception>();
        }

        public class MockCacheService : CacheServiceBase, ICacheService
        {
            private readonly ConcurrentDictionary<string, MockCacheItem> _cache;

            public MockCacheService()
            {
                _cache = new ConcurrentDictionary<string, MockCacheItem>();
            }

            public void Set(string key, object value)
            {
                _cache.TryAdd(key, new MockCacheItem(value));
            }

            public void SetExpiring(string key, object value, TimeSpan expiration, bool rollExpirationOnAccess = false)
            {
                throw new NotImplementedException();
            }

            public void SetSingleUse(string key, object value)
            {
                throw new NotImplementedException();
            }

            public void SetSingleUseExpiring(string key, object value, TimeSpan expiration, bool rollExpirationOnAccess = false)
            {
                throw new NotImplementedException();
            }

            public override bool TryGet<T>(string key, out T value)
            {
                if (!_cache.TryGetValue(key, out MockCacheItem cacheItem) || DateTime.Now > cacheItem.Expiration)
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
                throw new NotImplementedException();
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
}
