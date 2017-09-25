using System.Threading.Tasks;

namespace Utilitarian.Migrations.Interfaces
{
    public interface IDatabaseUtilityService
    {
        Task<bool> DatabaseExists();

        Task DropDatabase();
    }
}
