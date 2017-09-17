using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Utilitarian.Data.MongoDB;
using Utilitarian.Migrations.Interfaces;
using Utilitarian.Migrations.Models;
using Utilitarian.Settings;

namespace Utilitarian.Migrations.Repositories
{
    public class MongoVersionRepository : MongoDbRepositoryBase, IVersionRepository
    {
        public string DatabaseType => "MongoDb";

        public IMongoCollection<MongoDbVersionRecord> VersionRecords => GetCollection<MongoDbVersionRecord>("versionRecords");

        public MongoVersionRepository(string databaseName, IConnectionStringProvider connectionStringProvider)
            : base(databaseName, connectionStringProvider)
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

        public async Task<VersionRecord> MarkVersionRecordComplete(string migrationTopic, double version)
        {
            await Task.Run(() => { });
            throw new System.NotImplementedException();
        }

        public async Task<VersionRecord> MarkVersionRecordIncomplete(string migrationTopic, double version)
        {
            await Task.Run(() => { });
            throw new System.NotImplementedException();
        }

        public async Task DeleteVersionRecord(string migrationTopic, double version)
        {
            await Task.Run(() => { });
            throw new System.NotImplementedException();
        }
    }
}
