using System;
using System.Collections.Generic;
using System.Linq;
using Utilitarian.Migrations.Interfaces;

namespace Utilitarian.Migrations.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly IEnumerable<IVersionRepository> _versionRepositories;

        private readonly IEnumerable<IDatabaseUtilityService> _databaseUtilityServices;

        public RepositoryService(IEnumerable<IVersionRepository> versionRepositories, IEnumerable<IDatabaseUtilityService> databaseUtilityServices)
        {
            _versionRepositories = versionRepositories;
            _databaseUtilityServices = databaseUtilityServices;
        }

        public IVersionRepository GetVersionRepository(string databaseType, string databaseName, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(databaseType)) throw new ArgumentException("databaseType can not be blank", nameof(databaseType));
            if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException("databaseName can not be blank", nameof(databaseName));
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("connectionString can not be blank", nameof(connectionString));

            return _versionRepositories.SingleOrDefault(s => string.Equals(s.DatabaseType, databaseType, StringComparison.CurrentCultureIgnoreCase));
        }

        public IDatabaseUtilityService GetDatabaseUtilityRepository(string databaseType, string databaseName, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(databaseType)) throw new ArgumentException("databaseType can not be blank", nameof(databaseType));
            if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException("databaseName can not be blank", nameof(databaseName));
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("connectionString can not be blank", nameof(connectionString));

            return _databaseUtilityServices.SingleOrDefault(s => string.Equals(s.DatabaseType, databaseType, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
