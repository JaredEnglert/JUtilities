using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Utilitarian.Assemblies;
using Utilitarian.Migrations.Interfaces;
using Utilitarian.Migrations.Models;

namespace Utilitarian.Migrations.Services
{
    public class MigrationService : IMigrationService
    {
        private readonly string _migrationTopic;

        private readonly IAssemblyService _assemblyService;

        private readonly IVersionRepository _versionRepository;

        public MigrationService(string migrationTopic, IVersionRepository versionRepository, IAssemblyService assemblyService)
        {
            _migrationTopic = migrationTopic;
            _versionRepository = versionRepository;

            _assemblyService = assemblyService;

            Log.Logger.Information("Migration Service Initialized{0}{0}Migration Topic: {1}{0}Database Type: {1}{0}{0}============================================================={0}", Environment.NewLine, _migrationTopic,
                _versionRepository.DatabaseType);
        }

        public MigrationService(string migrationTopic, IVersionRepository versionRepository)
        {
            _migrationTopic = migrationTopic;
            _versionRepository = versionRepository;

            _assemblyService = new AssemblyService();

            Log.Logger.Information("Migration Service Initialized{0}{0}Migration Topic: {1}{0}Database Type: {1}{0}{0}============================================================={0}", Environment.NewLine, _migrationTopic,
                _versionRepository.DatabaseType);
        }

        public async Task MigrateUpPreRelease(IEnumerable<double> versions = null)
        {
            await InitializeVersionTable();

            var records = await _versionRepository.GetVersionRecords(_migrationTopic);

            var migrations = _assemblyService.GetAllImplementations<Migration>()
                .Where(m => records.All(r => !r.Version.Equals(m.Version)))
                .OrderBy(m => m.Version)
                .ToList();

            if (versions != null) migrations = migrations.Where(m => versions.Contains(m.Version)).ToList();

            var stopwatch = new Stopwatch();

            foreach (var migration in migrations)
            {
                Log.Logger.Information("Starting execution of MigrateUpPreRelease{0}Version: {1}, Description: {2}", Environment.NewLine, migration.Version, migration.Description);

                stopwatch.Restart();

                await migration.MigrateUpPreRelease();
                await _versionRepository.InsertVersionRecord(migration.MigrationTopic, migration.Version, migration.Description);

                stopwatch.Stop();

                Log.Logger.Information("MigrateUpPreRelease Complete{0}Version: {1}, Seconds: {2}", Environment.NewLine, migration.Version, stopwatch.ElapsedMilliseconds / 1000);
            }
        }

        public async Task MigrateUpPostRelease(IEnumerable<double> versions = null)
        {
            await InitializeVersionTable();

            var records = (await _versionRepository.GetVersionRecords(_migrationTopic)).ToList();

            var migrations = _assemblyService.GetAllImplementations<Migration>()
                .Where(m => records.Any(r => r.Version.Equals(m.Version) && r.MigrateUpPostReleaseRan == null))
                .OrderBy(m => m.Version)
                .ToList();

            if (versions != null) migrations = migrations.Where(m => versions.Contains(m.Version)).ToList();

            var stopwatch = new Stopwatch();

            foreach (var migration in migrations)
            {
                Log.Logger.Information("Starting execution of MigrateUpPostRelease{0}Version: {1}, Description: {2}", Environment.NewLine, migration.Version, migration.Description);

                stopwatch.Restart();

                await migration.MigrateUpPostRelease();
                await _versionRepository.MarkVersionRecordComplete(migration.MigrationTopic, migration.Version);

                stopwatch.Stop();

                Log.Logger.Information("MigrateUpPostRelease Complete{0}Version: {1}, Seconds: {2}", Environment.NewLine, migration.Version, stopwatch.ElapsedMilliseconds / 1000);
            }
        }

        public async Task MigrateDownPreRollback(IEnumerable<double> versions = null)
        {
            await InitializeVersionTable();

            var records = (await _versionRepository.GetVersionRecords(_migrationTopic)).ToList();

            var migrations = _assemblyService.GetAllImplementations<Migration>()
                .Where(m => records.Any(r => r.Version.Equals(m.Version) && r.MigrateUpPostReleaseRan != null))
                .OrderBy(m => m.Version)
                .ToList();

            if (versions != null) migrations = migrations.Where(m => versions.Contains(m.Version)).ToList();

            var stopwatch = new Stopwatch();

            foreach (var migration in migrations)
            {
                Log.Logger.Information("Starting execution of MigrateDownPreRollback{0}Version: {1}, Description: {2}", Environment.NewLine, migration.Version, migration.Description);

                stopwatch.Restart();

                await migration.MigrateDownPreRollback();
                await _versionRepository.MarkVersionRecordIncomplete(migration.MigrationTopic, migration.Version);

                stopwatch.Stop();

                Log.Logger.Information("MigrateDownPreRollback Complete{0}Version: {1}, Seconds: {2}", Environment.NewLine, migration.Version, stopwatch.ElapsedMilliseconds / 1000);
            }
        }

        public async Task MigrateDownPostRollback(IEnumerable<double> versions = null)
        {
            await InitializeVersionTable();

            var records = (await _versionRepository.GetVersionRecords(_migrationTopic)).ToList();

            var migrations = _assemblyService.GetAllImplementations<Migration>()
                .Where(m => records.Any(r => r.Version.Equals(m.Version) && r.MigrateUpPostReleaseRan == null))
                .OrderBy(m => m.Version)
                .ToList();

            if (versions != null) migrations = migrations.Where(m => versions.Contains(m.Version)).ToList();

            var stopwatch = new Stopwatch();

            foreach (var migration in migrations)
            {
                Log.Logger.Information("Starting execution of MigrateDownPostRollback{0}Version: {1}, Description: {2}", Environment.NewLine, migration.Version, migration.Description);

                stopwatch.Restart();

                await migration.MigrateDownPostRollback();
                await _versionRepository.DeleteVersionRecord(migration.MigrationTopic, migration.Version);

                stopwatch.Stop();

                Log.Logger.Information("MigrateDownPostRollback Complete{0}Version: {1}, Seconds: {2}", Environment.NewLine, migration.Version, stopwatch.ElapsedMilliseconds / 1000);
            }
        }

        private async Task InitializeVersionTable()
        {
            await _versionRepository.InitializeVersionTable();
        }
    }
}
