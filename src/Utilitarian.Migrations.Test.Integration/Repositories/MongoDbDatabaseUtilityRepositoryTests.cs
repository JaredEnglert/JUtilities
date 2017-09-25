using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Utilitarian.Migrations.Models;
using Utilitarian.Migrations.Repositories.MongoDb;
using Utilitarian.Migrations.Test.Integration.Factories;
using Utilitarian.Settings;

namespace Utilitarian.Migrations.Test.Integration.Repositories
{
    [TestClass]
    public class MongoDbDatabaseUtilityRepositoryTests
    {
        private const string DatabaseName = "Mongo";

        private MongoDbDatabaseUtilityRepository _mongoDbDatabaseUtilityRepository;

        public IMongoCollection<MongoDbVersionRecord> TestCollection => _mongoDbDatabaseUtilityRepository.GetCollection<MongoDbVersionRecord>("testCollection");

        #region TestInitialize and TestCleanup

        [TestInitialize]
        public void TestInitialize()
        {
            _mongoDbDatabaseUtilityRepository = GetMongoDbDatabaseUtilityRepository();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            var exists = await _mongoDbDatabaseUtilityRepository.DatabaseExists();

            if (exists) await _mongoDbDatabaseUtilityRepository.DropDatabase();

            _mongoDbDatabaseUtilityRepository = null;
        }

        #endregion TestInitialize and TestCleanup
        
        #region DropDatabase

        [TestMethod]
        public async Task DropDatabase_DoesNotExist_ShouldExecute()
        {
            await _mongoDbDatabaseUtilityRepository.DropDatabase();
        }

        [TestMethod]
        public async Task DropDatabase_DoesExist_ShouldDropDatabase()
        {
            await LoadRecord(MongoDbVersionRecordFactory.Generate());

            await _mongoDbDatabaseUtilityRepository.DropDatabase();

            var exists = await _mongoDbDatabaseUtilityRepository.DatabaseExists();

            exists.Should().BeFalse();
        }

        #endregion DropDatabase

        #region DatabaseExists Method

        [TestMethod]
        public async Task DatabaseExists_NoDatabase_ShouldReturnFalse()
        {
            var exists = await _mongoDbDatabaseUtilityRepository.DatabaseExists();

            exists.Should().BeFalse();
        }

        [TestMethod]
        public async Task DatabaseExists_DatabaseExists_ShouldReturnTrue()
        {
            await LoadRecord(MongoDbVersionRecordFactory.Generate());

            var exists = await _mongoDbDatabaseUtilityRepository.DatabaseExists();

            exists.Should().BeTrue();
        }

        #endregion DatabaseExists Method

        #region Private Methods

        private static MongoDbDatabaseUtilityRepository GetMongoDbDatabaseUtilityRepository(IConnectionStringProvider connectionStringProvider = null)
        {
            if(connectionStringProvider == null) connectionStringProvider = GetConnectionStringProvider();

            return new MongoDbDatabaseUtilityRepository($"{DatabaseName}{Guid.NewGuid()}".Replace("-", string.Empty), connectionStringProvider.Get(DatabaseName));
        }

        private static IConnectionStringProvider GetConnectionStringProvider()
        {
            return new AppSettingsConnectionStringProvider();
        }

        private async Task LoadRecord(MongoDbVersionRecord mongoDbVersionRecord, bool isUpsert = true)
        {
            var filter = Builders<MongoDbVersionRecord>.Filter.Eq(r => r.Id, mongoDbVersionRecord.Id);
            var options = new FindOneAndReplaceOptions<MongoDbVersionRecord, MongoDbVersionRecord> { IsUpsert = isUpsert };

            await TestCollection.FindOneAndReplaceAsync(filter, mongoDbVersionRecord, options);
        }

        #endregion Private Methods
    }
}
