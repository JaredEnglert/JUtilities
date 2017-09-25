using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Utilitarian.Migrations.Models;
using Utilitarian.Migrations.Repositories.MongoDb;
using Utilitarian.Migrations.Test.Integration.Factories;
using Utilitarian.Settings;

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PossibleMultipleEnumeration

namespace Utilitarian.Migrations.Test.Integration.Repositories
{
    [TestClass]
    public class MongoDbVersionRepositoryTests
    {
        private const string DatabaseName = "Mongo";

        private const string MigrationTopic = "MigrationTopic";

        private MongoDbVersionRepository _mongoDbVersionRepository;

        private MongoDbDatabaseUtilityRepository _mongoDbDatabaseUtilityRepository;

        #region TestInitialize and TestCleanup

        [TestInitialize]
        public void TestInitialize()
        {
            _mongoDbVersionRepository = GetMongoDbVersionRepository();
            _mongoDbDatabaseUtilityRepository = GetMongoDbDatabaseUtilityRepository();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            var exists = await _mongoDbDatabaseUtilityRepository.DatabaseExists();

            if (exists) await _mongoDbDatabaseUtilityRepository.DropDatabase();

            _mongoDbVersionRepository = null;
            _mongoDbDatabaseUtilityRepository = null;
        }

        #endregion TestInitialize and TestCleanup

        #region InitializeVersionTable Method

        [TestMethod]
        public async Task InitializeVersionTable_ShouldExecute()
        {
            await _mongoDbVersionRepository.InitializeVersionTable();
        }

        [TestMethod]
        public async Task InitializeVersionTable_CallMoreThanOnce_ShouldExecute()
        {
            await _mongoDbVersionRepository.InitializeVersionTable();
            await _mongoDbVersionRepository.InitializeVersionTable();
        }

        #endregion InitializeVersionTable Method

        #region GetMigrationRecords Method

        [TestMethod]
        public async Task GetMigrationRecords_NoRecords_ShouldReturnNoRecords()
        {
            var migrationRecords = await _mongoDbVersionRepository.GetVersionRecords(MigrationTopic);

            migrationRecords.Should().NotBeNull();
            migrationRecords.Any().Should().BeFalse();
        }

        [TestMethod]
        public async Task GetMigrationRecords_MultipleTopics_ShouldCorrectRecords()
        {
            await LoadRecord(MongoDbVersionRecordFactory.Generate(MigrationTopic, 1));
            await LoadRecord(MongoDbVersionRecordFactory.Generate(MigrationTopic, 2));
            await LoadRecord(MongoDbVersionRecordFactory.Generate("NotThe" + MigrationTopic, 1));
            await LoadRecord(MongoDbVersionRecordFactory.Generate("NotThe" + MigrationTopic, 4));

            var migrationRecords = await _mongoDbVersionRepository.GetVersionRecords(MigrationTopic);

            migrationRecords.Should().NotBeNull();
            migrationRecords.Count().Should().Be(2);
            migrationRecords.Any(r => r.Version == 1).Should().BeTrue();
            migrationRecords.Any(r => r.Version == 2).Should().BeTrue();
        }

        #endregion GetMigrationRecords Method

        #region InsertVersionRecord Method

        [TestMethod]
        public async Task InsertVersionRecord_ShouldInsertRecord()
        {
            var versionRecord = await _mongoDbVersionRepository.InsertVersionRecord(MigrationTopic, 1, "Description");

            var migrationRecords = await _mongoDbVersionRepository.GetVersionRecords(MigrationTopic);

            migrationRecords.Should().NotBeNull();
            migrationRecords.Count().Should().Be(1);

            var migrationRecord = migrationRecords.Single();
            migrationRecord.MigrationTopic.Should().Be(versionRecord.MigrationTopic);
            migrationRecord.Version.Should().Be(1);
            migrationRecord.Description.Should().Be("Description");
        }

        #endregion InsertVersionRecord Method

        #region MarkVersionRecordComplete Method

        [TestMethod]
        public async Task MarkVersionRecordComplete_ShouldCorrectRecordComplete()
        {
            await LoadRecord(MongoDbVersionRecordFactory.Generate(MigrationTopic, 1));
            await LoadRecord(MongoDbVersionRecordFactory.Generate(MigrationTopic, 2));
            await LoadRecord(MongoDbVersionRecordFactory.Generate("NotThe" + MigrationTopic, 1));

            await _mongoDbVersionRepository.MarkVersionRecordComplete(MigrationTopic, 1);

            var versionRecords = (await _mongoDbVersionRepository.GetVersionRecords(MigrationTopic)).ToList();
            var otherVersionRecords = (await _mongoDbVersionRepository.GetVersionRecords("NotThe" + MigrationTopic)).ToList();

            versionRecords.Single(r => r.MigrationTopic == MigrationTopic && r.Version == 1).MigrateUpPostReleaseRan.Should().NotBeNull();
            versionRecords.Single(r => r.MigrationTopic == MigrationTopic && r.Version == 2).MigrateUpPostReleaseRan.Should().BeNull();
            otherVersionRecords.Single(r => r.MigrationTopic != MigrationTopic && r.Version == 1).MigrateUpPostReleaseRan.Should().BeNull();
        }

        #endregion MarkVersionRecordComplete Method

        #region MarkVersionRecordIncomplete Method

        [TestMethod]
        public async Task MarkVersionRecordIncomplete_ShouldCorrectRecordComplete()
        {
            await LoadRecord(MongoDbVersionRecordFactory.Generate(MigrationTopic, 1, migrateUpPostReleaseRan: DateTime.UtcNow));
            await LoadRecord(MongoDbVersionRecordFactory.Generate(MigrationTopic, 2, migrateUpPostReleaseRan: DateTime.UtcNow));
            await LoadRecord(MongoDbVersionRecordFactory.Generate("NotThe" + MigrationTopic, 1, migrateUpPostReleaseRan: DateTime.UtcNow));

            await _mongoDbVersionRepository.MarkVersionRecordIncomplete(MigrationTopic, 1);

            var versionRecords = (await _mongoDbVersionRepository.GetVersionRecords(MigrationTopic)).ToList();
            var otherVersionRecords = (await _mongoDbVersionRepository.GetVersionRecords("NotThe" + MigrationTopic)).ToList();

            versionRecords.Single(r => r.MigrationTopic == MigrationTopic && r.Version == 1).MigrateUpPostReleaseRan.Should().BeNull();
            versionRecords.Single(r => r.MigrationTopic == MigrationTopic && r.Version == 2).MigrateUpPostReleaseRan.Should().NotBeNull();
            otherVersionRecords.Single(r => r.MigrationTopic != MigrationTopic && r.Version == 1).MigrateUpPostReleaseRan.Should().NotBeNull();
        }

        #endregion MarkVersionRecordIncomplete Method

        #region DeleteVersionRecord Method

        [TestMethod]
        public async Task DeleteVersionRecord_ShouldCorrectRecord()
        {
            await LoadRecord(MongoDbVersionRecordFactory.Generate(MigrationTopic, 1));
            await LoadRecord(MongoDbVersionRecordFactory.Generate(MigrationTopic, 2));
            await LoadRecord(MongoDbVersionRecordFactory.Generate("NotThe" + MigrationTopic, 1));

            await _mongoDbVersionRepository.DeleteVersionRecord(MigrationTopic, 1);

            var versionRecords = (await _mongoDbVersionRepository.GetVersionRecords(MigrationTopic)).ToList();
            var otherVersionRecords = (await _mongoDbVersionRepository.GetVersionRecords("NotThe" + MigrationTopic)).ToList();

            versionRecords.SingleOrDefault(r => r.MigrationTopic == MigrationTopic && r.Version == 1).Should().BeNull();
            versionRecords.SingleOrDefault(r => r.MigrationTopic == MigrationTopic && r.Version == 2).Should().NotBeNull();
            otherVersionRecords.SingleOrDefault(r => r.MigrationTopic != MigrationTopic && r.Version == 1).Should().NotBeNull();
        }

        #endregion DeleteVersionRecord Method

        #region Private Methods

        private static MongoDbVersionRepository GetMongoDbVersionRepository(IConnectionStringProvider connectionStringProvider = null)
        {
            if (connectionStringProvider == null) connectionStringProvider = GetConnectionStringProvider();

            return new MongoDbVersionRepository($"{DatabaseName}{Guid.NewGuid()}".Replace("-", string.Empty), connectionStringProvider.Get(DatabaseName));
        }

        private static MongoDbDatabaseUtilityRepository GetMongoDbDatabaseUtilityRepository(IConnectionStringProvider connectionStringProvider = null)
        {
            if (connectionStringProvider == null) connectionStringProvider = GetConnectionStringProvider();

            return new MongoDbDatabaseUtilityRepository($"{DatabaseName}{Guid.NewGuid()}".Replace("-", string.Empty), connectionStringProvider.Get(DatabaseName));
        }

        private static IConnectionStringProvider GetConnectionStringProvider()
        {
            return new AppSettingsConnectionStringProvider();
        }

        private async Task LoadRecord(MongoDbVersionRecord mongoDbVersionRecord, bool isUpsert = true)
        {
            var filter = Builders<MongoDbVersionRecord>.Filter.Eq(r => r.Id, mongoDbVersionRecord.Id);
            var options = new FindOneAndReplaceOptions<MongoDbVersionRecord, MongoDbVersionRecord> {IsUpsert = isUpsert};

            await _mongoDbVersionRepository.VersionRecords.FindOneAndReplaceAsync(filter, mongoDbVersionRecord, options);
        }

        #endregion Private Methods
    }
}
