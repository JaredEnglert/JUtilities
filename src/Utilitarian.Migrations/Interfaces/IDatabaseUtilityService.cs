using System.Threading.Tasks;

namespace Utilitarian.Migrations.Interfaces
{
    public interface IDatabaseUtilityService
    {
        string DatabaseType { get; }

        Task<bool> DatabaseExists();

        Task DropDatabase();
    }
}
