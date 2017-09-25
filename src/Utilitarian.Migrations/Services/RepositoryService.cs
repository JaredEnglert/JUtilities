using System;
using Utilitarian.Migrations.Interfaces;
using Utilitarian.Migrations.Repositories.MongoDb;

namespace Utilitarian.Migrations.Services
{
    public class RepositoryService : IRepositoryService
    {
        public IVersionRepository GetVersionRepository(string databaseType, string databaseName, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(databaseType)) throw new ArgumentException("databaseType can not be blank", nameof(databaseType));
            if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException("databaseName can not be blank", nameof(databaseName));
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("connectionString can not be blank", nameof(connectionString));

            switch (databaseType.ToUpper())
            {
                case "MONGODB": return new MongoDbVersionRepository(databaseName, connectionString);
                default: throw new NotImplementedException($"No version repository found for database Type: {databaseType}");
            }
        }

        public IDatabaseUtilityService GetDatabaseUtilityRepository(string databaseType, string databaseName, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(databaseType)) throw new ArgumentException("databaseType can not be blank", nameof(databaseType));
            if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException("databaseName can not be blank", nameof(databaseName));
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("connectionString can not be blank", nameof(connectionString));

            switch (databaseType.ToUpper())
            {
                case "MONGODB": return new MongoDbDatabaseUtilityRepository(databaseName, connectionString);
                default: throw new NotImplementedException($"No database utility repository found for database Type: {databaseType}");
            }
        }
    }
}
