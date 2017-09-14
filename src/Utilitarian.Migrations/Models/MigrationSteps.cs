namespace Utilitarian.Migrations.Models
{
    public enum MigrationSteps
    {
        MigrateUpPreRelease = 1,

        MigrateUpPostRelease = 2,

        MigrateDownPreRollback = 3,

        MigrateDownPostRollback = 4
    }
}
