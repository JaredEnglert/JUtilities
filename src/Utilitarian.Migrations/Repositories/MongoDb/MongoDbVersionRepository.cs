using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Utilitarian.Data.MongoDB;
using Utilitarian.Migrations.Interfaces;
using Utilitarian.Migrations.Models;

namespace Utilitarian.Migrations.Repositories.MongoDb
{
    public class MongoDbVersionRepository : MongoDbRepositoryBase, IVersionRepository
    {
        public string DatabaseType => "MongoDb";

        public IMongoCollection<MongoDbVersionRecord> VersionRecords => GetCollection<MongoDbVersionRecord>("versionRecords");

        public MongoDbVersionRepository(string databaseName, string connectionString)
            : base(databaseName, connectionString)
        {
        }

        public async Task InitializeVersionTable()
        {
            await Task.Run(() => { });
        }

        public async Task<IEnumerable<VersionRecord>> GetVersionRecords(string migrationTopic)
        {
            return await VersionRecords.Find(r => r.MigrationTopic == migrationTopic).ToListAsync();
        }

        public async Task<VersionRecord> InsertVersionRecord(string migrationTopic, double version, string description)
        {
            var versionRecord = new MongoDbVersionRecord
            {
                Id = ObjectId.GenerateNewId(),
                MigrationTopic = migrationTopic,
                Version = version,
                Description = description,
                MigrateUpPreReleaseRan = DateTime.UtcNow
            };

            await VersionRecords.InsertOneAsync(versionRecord);

            return versionRecord;
        }

        public async Task MarkVersionRecordComplete(string migrationTopic, double version)
        {
            var filter = Builders<MongoDbVersionRecord>.Filter
                .Where(r => r.MigrationTopic == migrationTopic && r.Version == version);
            
            var update = new UpdateDefinitionBuilder<MongoDbVersionRecord>()
                .Set(r => r.MigrateUpPostReleaseRan, DateTime.UtcNow);

            await VersionRecords.UpdateOneAsync(filter, update);
        }

        public async Task MarkVersionRecordIncomplete(string migrationTopic, double version)
        {
            var filter = Builders<MongoDbVersionRecord>.Filter
                .Where(r => r.MigrationTopic == migrationTopic && r.Version == version);

            var update = new UpdateDefinitionBuilder<MongoDbVersionRecord>()
                .Set(r => r.MigrateUpPostReleaseRan, null);

            await VersionRecords.UpdateOneAsync(filter, update);
        }

        public async Task DeleteVersionRecord(string migrationTopic, double version)
        {
            var filter = Builders<MongoDbVersionRecord>.Filter
                .Where(r => r.MigrationTopic == migrationTopic && r.Version == version);

            await VersionRecords.DeleteOneAsync(filter);
        }
    }
}
