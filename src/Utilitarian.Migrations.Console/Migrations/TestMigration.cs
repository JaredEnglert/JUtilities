using System.Threading.Tasks;
using Utilitarian.Migrations.Models;

namespace Utilitarian.Migrations.Console.Migrations
{
    public class TestMigration : Migration
    {
        public TestMigration() 
            : base("FirstTopic", 12345678901234, "My First Test Migration")
        {
        }

        public override async Task MigrateUpPreRelease()
        {
            await Task.Delay(2134);
        }

        public override async Task MigrateUpPostRelease()
        {
            await Task.Delay(2134);
        }

        public override async Task MigrateDownPreRollback()
        {
            await Task.Delay(2134);
        }

        public override async Task MigrateDownPostRollback()
        {
            await Task.Delay(2134);
        }
    }
}
