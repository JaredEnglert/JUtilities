using System.Collections.Generic;
using System.Threading.Tasks;

namespace Utilitarian.Migrations.Interfaces
{
    public interface IMigrationService
    {
        Task MigrateUpPreRelease(IEnumerable<double> versions = null);

        Task MigrateUpPostRelease(IEnumerable<double> versions = null);

        Task MigrateDownPreRollback(IEnumerable<double> versions = null);

        Task MigrateDownPostRollback(IEnumerable<double> versions = null);
    }
}
