using System;
using MongoDB.Bson;
using Utilitarian.Migrations.Models;

namespace Utilitarian.Migrations.Test.Integration.Factories
{
    public static class MongoDbVersionRecordFactory
    {
        public static MongoDbVersionRecord Generate(string migrationTopic = "MigrationTopic", double version = 1234, string description = "Description",
            DateTime? migrateUpPreReleaseRan = null, DateTime? migrateUpPostReleaseRan = null)
        {
            return new MongoDbVersionRecord
            {
                Id = ObjectId.GenerateNewId(),
                MigrationTopic = migrationTopic,
                Version = version,
                Description = description,
                MigrateUpPreReleaseRan = migrateUpPreReleaseRan ?? DateTime.UtcNow,
                MigrateUpPostReleaseRan = migrateUpPostReleaseRan
            };
        }
    }
}
