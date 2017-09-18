using System;
using Utilitarian.Migrations.Models;

namespace Utilitarian.Migrations.Test.Unit.Factories
{
    public static class VersionRecordFactory
    {
        public static VersionRecord Generate(string migrationTopic = "MigrationTopic", double version = 1234, string description = "Description",
            DateTime? migrateUpPreReleaseRan = null, DateTime? migrateUpPostReleaseRan = null)
        {
            return new VersionRecord
            {
                MigrationTopic = migrationTopic,
                Version = version,
                Description = description,
                MigrateUpPreReleaseRan = migrateUpPreReleaseRan ?? DateTime.UtcNow,
                MigrateUpPostReleaseRan = migrateUpPostReleaseRan
            };
        }
    }
}
