using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Utilitarian.Caching.Test.Unit.TestClasses
{
    [TestClass]
    public class CacheRepositoryTests
    {
        private const string Key = "TestKey";

        private const string Value = "TestValue";

        #region Get Method

        [TestMethod]
        public void Get_Cached_ShouldGetFromCache()
        {
            var store = GetStore();

            store
                .Stub(s => s.GetValue())
                .Return(Value);

            var cacheService = GetCacheService();

            cacheService
                .Stub(s => s.TryGet(Key, out string _))
                .OutRef(Value)
                .Return(true);

            using (var cacheRepository = GetCacheRepository(cacheService))
            {
                cacheRepository.Get(Key, store.GetValue);

                store.AssertWasNotCalled(s => s.GetValue());
            }
        }

        [TestMethod]
        public void Get_NotCached_ShouldGetFromDataStore()
        {
            var store = GetStore();

            store
                .Stub(s => s.GetValue())
                .Return(Value);

            var cacheService = GetCacheService();

            cacheService
                .Stub(s => s.TryGet(Key, out string _))
                .Return(false);

            using (var cacheRepository = GetCacheRepository(cacheService))
            {
                cacheRepository.Get(Key, store.GetValue);

                store.AssertWasCalled(s => s.GetValue());
            }
        }

        [TestMethod]
        public void Get_ShouldCacheValue()
        {
            var store = GetStore();

            store
                .Stub(s => s.GetValue())
                .Return(Value);

            var cacheService = GetCacheService();

            cacheService
                .Stub(s => s.TryGet(Key, out string _))
                .Return(false);

            using (var cacheRepository = new CacheRepository(cacheService))
            {
                cacheRepository.Get(Key, store.GetValue);

                cacheService.AssertWasCalled(s => s.SetExpiring(Arg<string>.Is.Anything, Arg<object>.Is.Anything, Arg<TimeSpan>.Is.Anything, Arg<bool>.Is.Anything));
            }
        }

        #endregion Get Method

        private static CacheRepository GetCacheRepository(ICacheService cacheService = null)
        {
            if (cacheService == null) cacheService = GetCacheService();

            return new CacheRepository(cacheService);
        }

        private static ICacheService GetCacheService()
        {
            return MockRepository.GenerateMock<ICacheService>();
        }

        private static IStore GetStore()
        {
            return MockRepository.GenerateMock<IStore>();
        }

        public interface IStore
        {
            string GetValue();
        }
    }
}
