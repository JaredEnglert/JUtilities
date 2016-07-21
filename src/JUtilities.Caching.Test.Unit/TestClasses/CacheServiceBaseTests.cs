using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JUtilities.Caching.Test.Unit.Mocks;

namespace JUtilities.Caching.Test.Unit.TestClasses
{
    [TestClass]
    public class CacheServiceBaseTests
    {
        private const string Key = "TestKey";

        private const string Value = "TestValue";

        [TestMethod]
        public void ShouldReturnValueWhenCached()
        {
            var cacheService = new MockCacheService();
            cacheService.Set(Key, Value);

            cacheService.Get<string>(Key).ShouldBeEquivalentTo(Value);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenNotCached()
        {
            var cacheService = new MockCacheService();
            Action action = () => cacheService.Get<string>(Key);

            action.ShouldThrow<Exception>();
        }
    }
}
