using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Utilitarian.Migrations.Interfaces;
using Utilitarian.Migrations.Models;

namespace Utilitarian.Migrations.Services
{
    public class MigrationService : IMigrationService
    {
        private readonly IVersionRepository _versionRepository;

        private readonly List<Migration> _migrations;

        public MigrationService(IVersionRepository versionRepository, IEnumerable<Migration> migrations)
        {
            _versionRepository = versionRepository;
            _migrations = migrations.ToList();
        }

        public async Task MigrateUpPreRelease(string migrationTopic, IEnumerable<double> versions = null)
        {
            var list = (versions ?? new List<double>()).ToList();

            await _versionRepository.InitializeVersionTable();

            var records = await _versionRepository.GetVersionRecords(migrationTopic);

            var migrations = _migrations
                .Where(m => records.All(r => !r.Version.Equals(m.Version)))
                .OrderBy(m => m.Version)
                .ToList();

            if (list.Any()) migrations = migrations.Where(m => list.Contains(m.Version)).ToList();

            var stopwatch = new Stopwatch();

            foreach (var migration in migrations)
            {
                Log.Information("Running Migration.MigrateUpPreRelease{0}{0}\tVersion: {1},{0}\tDescription: {2}{0}", Environment.NewLine, migration.Version, migration.Description);

                stopwatch.Restart();

                await migration.MigrateUpPreRelease();
                await _versionRepository.InsertVersionRecord(migration.MigrationTopic, migration.Version, migration.Description);

                stopwatch.Stop();

                Log.Information("Migration.MigrateUpPreRelease Complete for Version: {0} ({1}s)", migration.Version, stopwatch.Elapsed.TotalSeconds);
            }
        }

        public async Task MigrateUpPostRelease(string migrationTopic, IEnumerable<double> versions = null)
        {
            var list = (versions ?? new List<double>()).ToList();

            await _versionRepository.InitializeVersionTable();

            var records = (await _versionRepository.GetVersionRecords(migrationTopic)).ToList();

            var migrations = _migrations
                .Where(m => records.Any(r => r.Version.Equals(m.Version) && r.MigrateUpPostReleaseRan == null))
                .OrderBy(m => m.Version)
                .ToList();

            if (list.Any()) migrations = migrations.Where(m => list.Contains(m.Version)).ToList();

            var stopwatch = new Stopwatch();

            foreach (var migration in migrations)
            {
                Log.Information("Running Migration.MigrateUpPostRelease{0}{0}\tVersion: {1},{0}\tDescription: {2}{0}", Environment.NewLine, migration.Version, migration.Description);

                stopwatch.Restart();

                await migration.MigrateUpPostRelease();
                await _versionRepository.MarkVersionRecordComplete(migration.MigrationTopic, migration.Version);

                stopwatch.Stop();

                Log.Information("Migration.MigrateUpPostRelease Complete for Version: {0} ({1}s)", migration.Version, stopwatch.Elapsed.TotalSeconds);
            }
        }

        public async Task MigrateDownPreRollback(string migrationTopic, IEnumerable<double> versions = null)
        {
            var list = (versions ?? new List<double>()).ToList();

            await _versionRepository.InitializeVersionTable();

            var records = (await _versionRepository.GetVersionRecords(migrationTopic)).ToList();

            var migrations = _migrations
                .Where(m => records.Any(r => r.Version.Equals(m.Version) && r.MigrateUpPostReleaseRan != null))
                .OrderBy(m => m.Version)
                .ToList();

            if (list.Any()) migrations = migrations.Where(m => list.Contains(m.Version)).ToList();

            var stopwatch = new Stopwatch();

            foreach (var migration in migrations)
            {
                Log.Information("Running Migration.MigrateDownPreRollback{0}{0}\tVersion: {1},{0}\tDescription: {2}{0}", Environment.NewLine, migration.Version, migration.Description);

                stopwatch.Restart();

                await migration.MigrateDownPreRollback();
                await _versionRepository.MarkVersionRecordIncomplete(migration.MigrationTopic, migration.Version);

                stopwatch.Stop();

                Log.Information("Migration.MigrateDownPreRollback Complete for Version: {0} ({1}s)", migration.Version, stopwatch.Elapsed.TotalSeconds);
            }
        }

        public async Task MigrateDownPostRollback(string migrationTopic, IEnumerable<double> versions = null)
        {
            var list = (versions ?? new List<double>()).ToList();

            await _versionRepository.InitializeVersionTable();

            var records = (await _versionRepository.GetVersionRecords(migrationTopic)).ToList();

            var migrations = _migrations
                .Where(m => records.Any(r => r.Version.Equals(m.Version) && r.MigrateUpPostReleaseRan == null))
                .OrderBy(m => m.Version)
                .ToList();

            if (list.Any()) migrations = migrations.Where(m => list.Contains(m.Version)).ToList();

            var stopwatch = new Stopwatch();

            foreach (var migration in migrations)
            {
                Log.Information("Running Migration.MigrateDownPostRollback{0}{0}\tVersion: {1},{0}\tDescription: {2}{0}", Environment.NewLine, migration.Version, migration.Description);

                stopwatch.Restart();

                await migration.MigrateDownPostRollback();
                await _versionRepository.DeleteVersionRecord(migration.MigrationTopic, migration.Version);

                stopwatch.Stop();

                Log.Information("Migration.MigrateDownPostRollback Complete for Version: {0} ({1}s)", migration.Version, stopwatch.Elapsed.TotalSeconds);
            }
        }
    }
}
