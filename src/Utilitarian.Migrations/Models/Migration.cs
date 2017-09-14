using System.Threading.Tasks;

namespace Utilitarian.Migrations.Models
{
    public abstract class Migration
    {
        public string MigrationTopic { get; }

        public double Version { get; }

        public string Description { get; }

        protected Migration(string migrationTopic, double version, string description)
        {
            MigrationTopic = migrationTopic;
            Version = version;
            Description = description;
        }

        public abstract Task MigrateUpPreRelease();

        public abstract Task MigrateUpPostRelease();

        public abstract Task MigrateDownPreRollback();

        public abstract Task MigrateDownPostRollback();
    }
}
