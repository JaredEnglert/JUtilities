namespace Utilitarian.Migrations.Interfaces
{
    public interface IRepositoryService
    {
        IVersionRepository GetVersionRepository(string databaseType, string databaseName, string connectionString);
        
        IDatabaseUtilityService GetDatabaseUtilityRepository(string databaseType, string databaseName, string connectionString);
    }
}
