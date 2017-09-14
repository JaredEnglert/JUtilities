using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilitarian.Migrations.Interfaces;
using Utilitarian.Migrations.Models;

namespace Utilitarian.Migrations.Repositories
{
    internal class VersionRepository : IVersionRepository
    {
        public DatabaseType DatabaseType { get; }

        public VersionRepository(DatabaseType databaseType)
        {
            DatabaseType = databaseType;
        }

        public async Task InitializeVersionTable()
        {
            await Task.Delay(1);

            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MigrationRecord>> GetMigrationRecords(string migrationName)
        {
            await Task.Delay(1);

            throw new NotImplementedException();
        }

        public async Task InsertMigrationRecord(string migrationTopic, double version, string description)
        {
            //var migrationRecord = new MigrationRecord
            //{
            //    MigrationTopic = migrationTopic,
            //    Version = version,
            //    Description = description,
            //    MigrateUpPreReleaseRan = DateTime.UtcNow,
            //    MigrateUpPostReleaseRan = null
            //};

            await Task.Delay(1);

            throw new NotImplementedException();
        }

        public async Task MarkMigrationRecordComplete(string migrationTopic, double version)
        {
            await Task.Delay(1);

            throw new NotImplementedException();
        }

        public async Task MarkMigrationRecordIncomplete(string migrationTopic, double version)
        {
            await Task.Delay(1);

            throw new NotImplementedException();
        }

        public async Task DeleteMigrationRecord(string migrationTopic, double version)
        {
            await Task.Delay(1);

            throw new NotImplementedException();
        }
    }
}
