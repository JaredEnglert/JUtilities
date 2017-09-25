using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyConsole;
using Serilog;
using Utilitarian.Migrations.Interfaces;
using Utilitarian.Migrations.Models;
using Utilitarian.Migrations.Services;
using Utilitarian.Settings;

namespace Utilitarian.Migrations.Console.Commands
{
    public class MigrateUpPreReleaseCommand : ConsoleCommand
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        private readonly IRepositoryService _repositoryService;

        private readonly IEnumerable<Migration> _migrations;

        private string _databaseType;

        private string _databaseName;

        private string _migrationTopic;

        private List<double> _versions;

        public MigrateUpPreReleaseCommand(IConnectionStringProvider connectionStringProvider, IRepositoryService repositoryService, IEnumerable<Migration> migrations)
        {
            _connectionStringProvider = connectionStringProvider;
            _repositoryService = repositoryService;
            _migrations = migrations;

            IsCommand("migrate-up-pre-release", "Runs the MigrateUpPreRelease method on all eligible migrations. It is Step 1, and used at the beginning of a release.");

            HasRequiredOption("dt|databaseType=", "Type of the database where migration is being run.", ty => _databaseType = ty);
            HasRequiredOption("dn|databaseName=", "Name of the database where migration is being run.", n => _databaseName = n);
            HasRequiredOption("t|topic=", "Topic of the migration being run.", to => _migrationTopic = to);

            HasOption("v|versions=", "Whitelist of versions to be migrated.", versions => _versions = versions.Split(';').Select(double.Parse).ToList());
        }

        public override int Run(string[] remainingArguments)
        {
            RunAsync().Wait();

            return 1;
        }

        private async Task RunAsync()
        {
            var versionString = _versions != null && _versions.Any()
                ? string.Format("\tVersion Whitelist:{0}{0}\t\t{1}{0}", Environment.NewLine, string.Join(Environment.NewLine + "\t\t", _versions))
                : string.Empty;

            Log.Information("Starting MigrationService.MigrateUpPreRelease{0}{0}\tDatabase Type: {1}{0}\tDatabase Name: {2}{0}\tMigration Topic: {3}{0}{4}", 
                Environment.NewLine, _databaseType, _databaseName, _migrationTopic, versionString);

            var versionRepo = _repositoryService.GetVersionRepository(_databaseType, _databaseName, _connectionStringProvider.Get(_databaseName));

            Log.Information("Using Version Repository {0}", versionRepo.GetType().Name);

            var migrationService = new MigrationService(versionRepo, _migrations);

            await migrationService.MigrateUpPreRelease(_migrationTopic, _versions);

            Log.Information("MigrationService.MigrateUpPreRelease Complete");
        }
    }
}
