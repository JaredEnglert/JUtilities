using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Utilitarian.Data.MongoDB.Test.Integration.Repositories;

namespace Utilitarian.Data.MongoDB.Test.Integration
{
    [TestClass]
    public class MongoDbRepositoryBaseTests
    {
        private TestMongoDbRepositoryBase _testMongoDbRepositoryBase;

        #region TestInitialize and TestCleanup

        [TestInitialize]
        public void TestInitialize()
        {
            _testMongoDbRepositoryBase = GetTestMongoDbRepositoryBase();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            var exists = await _testMongoDbRepositoryBase.DatabaseExists();

            if (exists) _testMongoDbRepositoryBase.MongoClient.DropDatabase(_testMongoDbRepositoryBase.DatabaseName);

            _testMongoDbRepositoryBase = null;
        }

        #endregion TestInitialize and TestCleanup

        #region DatabaseExists Method

        [TestMethod]
        public async Task DatabaseExists_NoDatabase_ShouldReturnFalse()
        {
            var exists = await _testMongoDbRepositoryBase.DatabaseExists();

            exists.Should().BeFalse();
        }

        [TestMethod]
        public async Task DatabaseExists_DatabaseExists_ShouldReturnTrue()
        {
            var client = _testMongoDbRepositoryBase.MongoClient;
            var database = client.GetDatabase(_testMongoDbRepositoryBase.DatabaseName);
            var collection = database.GetCollection<TestClass>("testCollection");

            await collection.InsertOneAsync(new TestClass());

            var exists = await _testMongoDbRepositoryBase.DatabaseExists();

            exists.Should().BeTrue();
        }

        #endregion DatabaseExists Method

        #region Private Method

        private static TestMongoDbRepositoryBase GetTestMongoDbRepositoryBase()
        {
            return new TestMongoDbRepositoryBase();
        }

        #endregion Private Method

        private class TestClass
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            // ReSharper disable once MemberCanBePrivate.Local
            public ObjectId Id { get; }

            public TestClass()
            {
                Id = ObjectId.GenerateNewId();
            }
        }
    }
}
