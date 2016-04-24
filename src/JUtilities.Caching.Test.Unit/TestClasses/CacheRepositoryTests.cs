using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JUtilities.Caching.Test.Unit.Mocks;

namespace JUtilities.Caching.Test.Unit.TestClasses
{
    [TestClass]
    public class CacheRepositoryTests
    {
        private const string Key = "TestKey";

        [TestMethod]
        public void ShouldGetFromDataStoreWhenNotCached()
        {
            var store = new Store();
            using (var cacheRepository = GetCacheRepository())
            {
                cacheRepository.Get(Key, store.GetString);

                store.TimesCalled.ShouldBeEquivalentTo(1);
            }
        }

        [TestMethod]
        public void ShouldOnlyGetFromDataStoreOnce()
        {
            var store = new Store();
            using (var cacheRepository = GetCacheRepository())
            {
                var value = cacheRepository.Get(Key, store.GetString);
                cacheRepository.Get(Key, store.GetString);
                cacheRepository.Get(Key, store.GetString);

                store.TimesCalled.ShouldBeEquivalentTo(1);
            }
        }

        [TestMethod]
        public void ShouldCacheValue()
        {
            var store = new Store();
            var cacheService = new MockCacheService();
            using (var cacheRepository = new CacheRepository(cacheService))
            {
                var value = cacheRepository.Get(Key, store.GetString);
                cacheService.Get<string>(Key).ShouldBeEquivalentTo(value);
            }
        }

        private CacheRepository GetCacheRepository()
        {
            return new CacheRepository(new MockCacheService());
        }

        private class Store
        {
            public int TimesCalled { get; set; }

            public Store()
            {
                TimesCalled = 0;
            }

            public string GetString()
            {
                TimesCalled++;

                return "SomeString";
            }
        }
    }
}
