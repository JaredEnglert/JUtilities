using System.Collections.Generic;
using System.Threading.Tasks;

namespace Utilitarian.Migrations.Interfaces
{
    public interface IMigrationService
    {
        Task MigrateUpPreRelease(string migrationTopic, IEnumerable<double> versions = null);

        Task MigrateUpPostRelease(string migrationTopic, IEnumerable<double> versions = null);

        Task MigrateDownPreRollback(string migrationTopic, IEnumerable<double> versions = null);

        Task MigrateDownPostRollback(string migrationTopic, IEnumerable<double> versions = null);
    }
}
