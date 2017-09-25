using System.Threading.Tasks;
using ManyConsole;
using Serilog;
using Utilitarian.Migrations.Interfaces;
using Utilitarian.Settings;

namespace Utilitarian.Migrations.Console.Commands
{
    public class DropDatabaseCommand : ConsoleCommand
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        private readonly IRepositoryService _repositoryService;

        private string _databaseType;

        private string _databaseName;

        public DropDatabaseCommand(IConnectionStringProvider connectionStringProvider, IRepositoryService repositoryService)
        {
            _connectionStringProvider = connectionStringProvider;
            _repositoryService = repositoryService;

            IsCommand("drop-database", "Drops the database.");

            HasRequiredOption("dt|databaseType=", "Type of the database where migration is being run.", ty => _databaseType = ty);
            HasRequiredOption("dn|databaseName=", "Name of the database where migration is being run.", n => _databaseName = n);
        }

        public override int Run(string[] remainingArguments)
        {
            RunAsync().Wait();

            return 1;
        }

        private async Task RunAsync()
        {
            Log.Information("Starting to Drop Database.");

            var versionRepo = _repositoryService.GetDatabaseUtilityRepository(_databaseType, _databaseName, _connectionStringProvider.Get(_databaseName));

            Log.Information("Using Database Utility Repository {0}", versionRepo.GetType().Name);

            await versionRepo.DropDatabase();

            Log.Information("Database Drop Complete");
        }
    }
}
