using System.Collections.Generic;
using System.Threading.Tasks;
using Utilitarian.Migrations.Models;

namespace Utilitarian.Migrations.Interfaces
{
    public interface IVersionRepository
    {
        string DatabaseType { get; }

        Task InitializeVersionTable();

        Task<IEnumerable<VersionRecord>> GetVersionRecords(string migrationName);

        Task<VersionRecord> InsertVersionRecord(string migrationTopic, double version, string description);

        Task MarkVersionRecordComplete(string migrationTopic, double version);

        Task MarkVersionRecordIncomplete(string migrationTopic, double version);

        Task DeleteVersionRecord(string migrationTopic, double version);
    }
}
