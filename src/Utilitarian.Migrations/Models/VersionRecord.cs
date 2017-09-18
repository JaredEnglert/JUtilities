using System;

namespace Utilitarian.Migrations.Models
{
    public class VersionRecord
    {
        public string MigrationTopic { get; set; }

        public double Version { get; set; }

        public string Description { get; set; }

        public DateTime MigrateUpPreReleaseRan { get; set; }

        public DateTime? MigrateUpPostReleaseRan { get; set; }
    }
}
