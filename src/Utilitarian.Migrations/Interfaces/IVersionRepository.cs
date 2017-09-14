using System.Collections.Generic;
using System.Threading.Tasks;
using Utilitarian.Migrations.Models;

namespace Utilitarian.Migrations.Interfaces
{
    public interface IVersionRepository
    {
        DatabaseType DatabaseType { get; }

        Task InitializeVersionTable();

        Task<IEnumerable<MigrationRecord>> GetMigrationRecords(string migrationName);

        Task InsertMigrationRecord(string migrationTopic, double version, string description);

        Task MarkMigrationRecordComplete(string migrationTopic, double version);

        Task MarkMigrationRecordIncomplete(string migrationTopic, double version);

        Task DeleteMigrationRecord(string migrationTopic, double version);
    }
}
