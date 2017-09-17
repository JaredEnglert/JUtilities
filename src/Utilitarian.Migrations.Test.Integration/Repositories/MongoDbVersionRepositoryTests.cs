using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Rhino.Mocks;
using Utilitarian.Migrations.Models;
using Utilitarian.Migrations.Repositories;
using Utilitarian.Migrations.Test.Integration.Factories;
using Utilitarian.Settings;

namespace Utilitarian.Migrations.Test.Integration.Repositories
{
    [TestClass]
    public class MongoDbVersionRepositoryTests
    {
        private const string DatabaseName = "MongoDbVersionRepositoryIntegrationTest";

        private const string MigrationTopic = "MigrationTopic";

        private MongoVersionRepository mongoVersionRepository;

        #region TestInitialize Setup

        [TestInitialize]
        public void TestInitialize()
        {
            mongoVersionRepository = GetMongoVersionRepository();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            var exists = await mongoVersionRepository.DatabaseExists();

            if (exists) mongoVersionRepository.MongoClient.DropDatabase(DatabaseName);

            mongoVersionRepository = null;
        }

        #endregion TestInitialize Setup

        #region InitializeVersionTable Method

        [TestMethod]
        public async Task InitializeVersionTable_ShouldExecute()
        {
            await mongoVersionRepository.InitializeVersionTable();
        }

        [TestMethod]
        public async Task InitializeVersionTable_CallMoreThanOnce_ShouldExecute()
        {
            await mongoVersionRepository.InitializeVersionTable();
            await mongoVersionRepository.InitializeVersionTable();
        }

        #endregion InitializeVersionTable Method

        #region GetMigrationRecords Method

        [TestMethod]
        public async Task GetMigrationRecords_NoRecords_ShouldReturnNoRecords()
        {
            var migrationRecords = await mongoVersionRepository.GetVersionRecords(MigrationTopic);

            migrationRecords.Should().NotBeNull();
            migrationRecords.Any().Should().BeFalse();
        }

        [TestMethod]
        public async Task GetMigrationRecords_MultipleTopics_ShouldCorrectRecords()
        {
            await LoadRecord(MongoDbVersionRecordFactory.Generate(MigrationTopic, 1));
            await LoadRecord(MongoDbVersionRecordFactory.Generate(MigrationTopic, 2));
            await LoadRecord(MongoDbVersionRecordFactory.Generate("NotThe" + MigrationTopic, 3));
            await LoadRecord(MongoDbVersionRecordFactory.Generate("NotThe" + MigrationTopic, 4));

            var migrationRecords = await mongoVersionRepository.GetVersionRecords(MigrationTopic);

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
            var versionRecord = await mongoVersionRepository.InsertVersionRecord(MigrationTopic, 1, "Description");

            var migrationRecords = await mongoVersionRepository.GetVersionRecords(MigrationTopic);

            migrationRecords.Should().NotBeNull();
            migrationRecords.Count().Should().Be(1);

            var migrationRecord = migrationRecords.Single();
            migrationRecord.MigrationTopic.Should().Be(versionRecord.MigrationTopic);
            migrationRecord.Version.Should().Be(1);
            migrationRecord.Description.Should().Be("Description");
        }

        #endregion InsertVersionRecord Method

        #region Private Methods

        private static MongoVersionRepository GetMongoVersionRepository(IConnectionStringProvider connectionStringProvider = null)
        {
            if (connectionStringProvider == null) connectionStringProvider = GetConnectionStringProvider();

            return new MongoVersionRepository(DatabaseName, connectionStringProvider);
        }

        private static IConnectionStringProvider GetConnectionStringProvider()
        {
            var connectionStringProvider = MockRepository.GenerateMock<IConnectionStringProvider>();

            connectionStringProvider.Stub(p => p.Get(DatabaseName)).Return("mongodb://localhost:27017");

            return connectionStringProvider;
        }

        private async Task LoadRecord(MongoDbVersionRecord mongoDbVersionRecord, bool isUpsert = true)
        {
            var filter = Builders<MongoDbVersionRecord>.Filter.Eq(r => r.Id, mongoDbVersionRecord.Id);
            var options = new FindOneAndReplaceOptions<MongoDbVersionRecord, MongoDbVersionRecord> {IsUpsert = isUpsert};

            await mongoVersionRepository.VersionRecords.FindOneAndReplaceAsync(filter, mongoDbVersionRecord, options);
        }

        #endregion Private Methods
    }
}
