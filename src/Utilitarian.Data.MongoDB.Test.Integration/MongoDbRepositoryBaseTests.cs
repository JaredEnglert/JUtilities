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
        private MockMongoDbRepositoryBase _mockMongoDbRepositoryBase;

        #region TestInitialize and TestCleanup

        [TestInitialize]
        public void TestInitialize()
        {
            _mockMongoDbRepositoryBase = GetMockMongoDbRepositoryBase();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            var exists = await _mockMongoDbRepositoryBase.DatabaseExists();

            if (exists) _mockMongoDbRepositoryBase.MongoClient.DropDatabase(_mockMongoDbRepositoryBase.DatabaseName);

            _mockMongoDbRepositoryBase = null;
        }

        #endregion TestInitialize and TestCleanup

        #region DatabaseExists Method

        [TestMethod]
        public async Task DatabaseExists_NoDatabase_ShouldReturnFalse()
        {
            var exists = await _mockMongoDbRepositoryBase.DatabaseExists();

            exists.Should().BeFalse();
        }

        [TestMethod]
        public async Task DatabaseExists_DatabaseExists_ShouldReturnTrue()
        {
            var client = _mockMongoDbRepositoryBase.MongoClient;
            var database = client.GetDatabase(_mockMongoDbRepositoryBase.DatabaseName);
            var collection = database.GetCollection<TestClass>("testCollection");

            await collection.InsertOneAsync(new TestClass());

            var exists = await _mockMongoDbRepositoryBase.DatabaseExists();

            exists.Should().BeTrue();
        }

        #endregion DatabaseExists Method

        #region Private Method

        private static MockMongoDbRepositoryBase GetMockMongoDbRepositoryBase()
        {
            return new MockMongoDbRepositoryBase();
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
