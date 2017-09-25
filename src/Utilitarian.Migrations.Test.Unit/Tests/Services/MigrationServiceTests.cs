using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Utilitarian.Migrations.Interfaces;
using Utilitarian.Migrations.Models;
using Utilitarian.Migrations.Services;
using Utilitarian.Migrations.Test.Unit.Factories;

// ReSharper disable ArgumentsStyleOther
// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable ArgumentsStyleNamedExpression

namespace Utilitarian.Migrations.Test.Unit.Tests.Services
{
    [TestClass]
    public class MigrationServiceTests
    {
        private const string MigrationTopic = "Unit Test";

        private const double Version = 123;

        #region MigrateUpPreRelease Method

        [TestMethod]
        public async Task MigrateUpPreRelease_NoMigrations_ShouldRunSuccessfully()
        {
            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new List<VersionRecord>().AsEnumerable()));

            versionRepository
                .Stub(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything))
                .Return(Task.FromResult(VersionRecordFactory.Generate()));

            var migrationService = GetMigrationService(versionRepository: versionRepository);

            await migrationService.MigrateUpPreRelease(MigrationTopic);

            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
        }
        
        [TestMethod]
        public async Task MigrateUpPreRelease_MigrationThatHasAlreadyBeenRan_ShouldNotMigrate()
        {
            var migration = GetMigration(MigrationTopic, Version);
            
            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new[] { VersionRecordFactory.Generate(MigrationTopic, Version, migrateUpPostReleaseRan: null) }.AsEnumerable()));

            versionRepository
                .Stub(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything))
                .Return(Task.FromResult(VersionRecordFactory.Generate()));

            var migrationService = GetMigrationService(versionRepository, new List<Migration> { migration });

            await migrationService.MigrateUpPreRelease(MigrationTopic);
            
            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));

            migration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            migration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            migration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            migration.AssertWasNotCalled(m => m.MigrateDownPostRollback());
        }

        [TestMethod]
        public async Task MigrateUpPreRelease_NewMigration_ShouldMigrate()
        {
            var migration = GetMigration(MigrationTopic, Version);

            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new List<VersionRecord>().AsEnumerable()));

            versionRepository
                .Stub(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything))
                .Return(Task.FromResult(VersionRecordFactory.Generate()));

            var migrationService = GetMigrationService(versionRepository, new List<Migration> { migration });

            await migrationService.MigrateUpPreRelease(MigrationTopic);

            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));

            migration.AssertWasCalled(m => m.MigrateUpPreRelease());
            migration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            migration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            migration.AssertWasNotCalled(m => m.MigrateDownPostRollback());
        }

        [TestMethod]
        public async Task MigrateUpPreRelease_Whitelist_ShouldOnlyMigrateIncludedVersions()
        {
            var goodMigration = GetMigration(MigrationTopic, Version);
            var badMigration = GetMigration(MigrationTopic, Version + 1);

            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new List<VersionRecord>().AsEnumerable()));

            versionRepository
                .Stub(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything))
                .Return(Task.FromResult(VersionRecordFactory.Generate()));

            var migrationService = GetMigrationService(versionRepository, new List<Migration> { goodMigration, badMigration });

            await migrationService.MigrateUpPreRelease(MigrationTopic, new[] { goodMigration.Version });

            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Equal(goodMigration.Version), Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Equal(badMigration.Version), Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));

            goodMigration.AssertWasCalled(m => m.MigrateUpPreRelease());
            goodMigration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            goodMigration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            goodMigration.AssertWasNotCalled(m => m.MigrateDownPostRollback());

            badMigration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            badMigration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            badMigration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            badMigration.AssertWasNotCalled(m => m.MigrateDownPostRollback());
        }

        [TestMethod]
        public async Task MigrateUpPreRelease_EmptyWhitelist_ShouldMigrate()
        {
            var migration = GetMigration(MigrationTopic, Version);

            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new List<VersionRecord>().AsEnumerable()));

            versionRepository
                .Stub(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything))
                .Return(Task.FromResult(VersionRecordFactory.Generate()));

            var migrationService = GetMigrationService(versionRepository, new List<Migration> { migration });

            await migrationService.MigrateUpPreRelease(MigrationTopic, new List<double>());

            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Equal(migration.Version), Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));

            migration.AssertWasCalled(m => m.MigrateUpPreRelease());
            migration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            migration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            migration.AssertWasNotCalled(m => m.MigrateDownPostRollback());
        }

        #endregion MigrateUpPreRelease Method

        #region MigrateUpPostRelease Method

        [TestMethod]
        public async Task MigrateUpPostRelease_NoMigrations_ShouldRunSuccessfully()
        {
            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new List<VersionRecord>().AsEnumerable()));

            versionRepository
                .Stub(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything))
                .Return(Task.FromResult(VersionRecordFactory.Generate()));

            var migrationService = GetMigrationService(versionRepository);

            await migrationService.MigrateUpPostRelease(MigrationTopic);

            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
        }
        
        [TestMethod]
        public async Task MigrateUpPostRelease_MigrationThatHasAlreadyBeenRan_ShouldNotMigrate()
        {
            var migration = GetMigration(MigrationTopic, Version);
            
            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new[] { VersionRecordFactory.Generate(MigrationTopic, Version, migrateUpPostReleaseRan: DateTime.UtcNow) }.AsEnumerable()));

            versionRepository
                .Stub(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything))
                .Return(Task.FromResult(VersionRecordFactory.Generate()));

            var migrationService = GetMigrationService(versionRepository, new List<Migration> { migration });

            await migrationService.MigrateUpPostRelease(MigrationTopic);

            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));

            migration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            migration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            migration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            migration.AssertWasNotCalled(m => m.MigrateDownPostRollback());
        }

        [TestMethod]
        public async Task MigrateUpPostRelease_NewMigration_ShouldNotMigrate()
        {
            var migration = GetMigration(MigrationTopic, Version);

            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new List<VersionRecord>().AsEnumerable()));

            versionRepository
                .Stub(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything))
                .Return(Task.FromResult(VersionRecordFactory.Generate()));

            var migrationService = GetMigrationService(versionRepository, new List<Migration> { migration });

            await migrationService.MigrateUpPostRelease(MigrationTopic);

            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));

            migration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            migration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            migration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            migration.AssertWasNotCalled(m => m.MigrateDownPostRollback());
        }

        [TestMethod]
        public async Task MigrateUpPostRelease_MigrationWithPreAndNoPost_ShouldMigrate()
        {
            var migration = GetMigration(MigrationTopic, Version);

            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new[] { VersionRecordFactory.Generate(MigrationTopic, Version, migrateUpPostReleaseRan: null) }.AsEnumerable()));

            versionRepository
                .Stub(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything))
                .Return(Task.FromResult(VersionRecordFactory.Generate()));

            var migrationService = GetMigrationService(versionRepository, new[] { migration });

            await migrationService.MigrateUpPostRelease(MigrationTopic);

            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));

            migration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            migration.AssertWasCalled(m => m.MigrateUpPostRelease());
            migration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            migration.AssertWasNotCalled(m => m.MigrateDownPostRollback());
        }

        [TestMethod]
        public async Task MigrateUpPostRelease_Whitelist_ShouldOnlyMigrateIncludedVersions()
        {
            var goodMigration = GetMigration(MigrationTopic, Version);
            var badMigration = GetMigration(MigrationTopic, Version + 1);

            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new[]
                {
                    VersionRecordFactory.Generate(MigrationTopic, Version, migrateUpPostReleaseRan: null),
                    VersionRecordFactory.Generate(MigrationTopic, Version + 1, migrateUpPostReleaseRan: null)
                }.AsEnumerable()));

            versionRepository
                .Stub(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything))
                .Return(Task.FromResult(VersionRecordFactory.Generate()));

            var migrationService = GetMigrationService(versionRepository, new List<Migration> { goodMigration, badMigration });

            await migrationService.MigrateUpPostRelease(MigrationTopic, new[] { goodMigration.Version });

            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Equal(goodMigration.Version)));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Equal(badMigration.Version)));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));

            goodMigration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            goodMigration.AssertWasCalled(m => m.MigrateUpPostRelease());
            goodMigration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            goodMigration.AssertWasNotCalled(m => m.MigrateDownPostRollback());

            badMigration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            badMigration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            badMigration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            badMigration.AssertWasNotCalled(m => m.MigrateDownPostRollback());
        }

        #endregion MigrateUpPostRelease Method

        #region MigrateDownPreRollback Method

        [TestMethod]
        public async Task MigrateDownPreRollback_NoMigrations_ShouldRunSuccessfully()
        {
            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new List<VersionRecord>().AsEnumerable()));

            versionRepository
                .Stub(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything))
                .Return(Task.FromResult(VersionRecordFactory.Generate()));

            var migrationService = GetMigrationService(versionRepository);

            await migrationService.MigrateDownPreRollback(MigrationTopic);

            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
        }
        
        [TestMethod]
        public async Task MigrateDownPreRollback_MigrationThatHasAlreadyBeenRan_ShouldNotRollback()
        {
            var migration = GetMigration(MigrationTopic, Version);
            
            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new[] { VersionRecordFactory.Generate(MigrationTopic, Version, migrateUpPostReleaseRan: null) }.AsEnumerable()));

            versionRepository
                .Stub(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything))
                .Return(Task.FromResult(VersionRecordFactory.Generate()));

            var migrationService = GetMigrationService(versionRepository, new List<Migration> { migration });

            await migrationService.MigrateDownPreRollback(MigrationTopic);
            
            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));

            migration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            migration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            migration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            migration.AssertWasNotCalled(m => m.MigrateDownPostRollback());
        }

        [TestMethod]
        public async Task MigrateDownPreRollback_NewMigration_ShouldNotRollback()
        {
            var migration = GetMigration(MigrationTopic, Version);

            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new List<VersionRecord>().AsEnumerable()));

            versionRepository
                .Stub(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything))
                .Return(Task.FromResult(VersionRecordFactory.Generate()));

            var migrationService = GetMigrationService(versionRepository, new List<Migration> { migration });

            await migrationService.MigrateDownPreRollback(MigrationTopic);

            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));

            migration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            migration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            migration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            migration.AssertWasNotCalled(m => m.MigrateDownPostRollback());
        }

        [TestMethod]
        public async Task MigrateDownPreRollback_MigrationWithPreAndPost_ShouldRollback()
        {
            var migration = GetMigration(MigrationTopic, Version);

            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new[] { VersionRecordFactory.Generate(MigrationTopic, Version, migrateUpPostReleaseRan: DateTime.UtcNow) }.AsEnumerable()));

            versionRepository
                .Stub(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything))
                .Return(Task.FromResult(VersionRecordFactory.Generate()));

            var migrationService = GetMigrationService(versionRepository, new[] { migration });

            await migrationService.MigrateDownPreRollback(MigrationTopic);
            
            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));

            migration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            migration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            migration.AssertWasCalled(m => m.MigrateDownPreRollback());
            migration.AssertWasNotCalled(m => m.MigrateDownPostRollback());
        }

        [TestMethod]
        public async Task MigrateDownPreRollback_Whitelist_ShouldOnlyRollbackIncludedVersions()
        {
            var goodMigration = GetMigration(MigrationTopic, Version);
            var badMigration = GetMigration(MigrationTopic, Version + 1);
            
            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new[]
                {
                    VersionRecordFactory.Generate(MigrationTopic, Version, migrateUpPostReleaseRan: DateTime.UtcNow),
                    VersionRecordFactory.Generate(MigrationTopic, Version + 1, migrateUpPostReleaseRan: DateTime.UtcNow)
                }.AsEnumerable()));

            versionRepository
                .Stub(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything))
                .Return(Task.FromResult(VersionRecordFactory.Generate()));

            var migrationService = GetMigrationService(versionRepository, new List<Migration> { goodMigration, badMigration });

            await migrationService.MigrateDownPreRollback(MigrationTopic, new[] { goodMigration.Version });

            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Equal(goodMigration.Version)));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Equal(badMigration.Version)));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));

            goodMigration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            goodMigration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            goodMigration.AssertWasCalled(m => m.MigrateDownPreRollback());
            goodMigration.AssertWasNotCalled(m => m.MigrateDownPostRollback());

            badMigration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            badMigration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            badMigration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            badMigration.AssertWasNotCalled(m => m.MigrateDownPostRollback());
        }

        #endregion MigrateDownPreRollback Method

        #region MigrateDownPostRollback Method

        [TestMethod]
        public async Task MigrateDownPostRollback_NoMigrations_ShouldRunSuccessfully()
        {
            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new List<VersionRecord>().AsEnumerable()));

            versionRepository
                .Stub(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything))
                .Return(Task.Run(() => { }));

            var migrationService = GetMigrationService(versionRepository);

            await migrationService.MigrateDownPostRollback(MigrationTopic);
            
            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
        }
        
        [TestMethod]
        public async Task MigrateDownPostRollback_NewMigration_ShouldNotRollback()
        {
            var migration = GetMigration(MigrationTopic, Version);
            
            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new List<VersionRecord>().AsEnumerable()));

            versionRepository
                .Stub(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything))
                .Return(Task.Run(() => { }));

            var migrationService = GetMigrationService(versionRepository, new List<Migration> { migration });

            await migrationService.MigrateDownPostRollback(MigrationTopic);
            
            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));

            migration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            migration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            migration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            migration.AssertWasNotCalled(m => m.MigrateDownPostRollback());
        }

        [TestMethod]
        public async Task MigrateDownPostRollback_MigrationWithPreAndNoPost_ShouldRollback()
        {
            var migration = GetMigration(MigrationTopic, Version);
            
            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new[] { VersionRecordFactory.Generate(MigrationTopic, Version, migrateUpPostReleaseRan: null) }.AsEnumerable()));

            versionRepository
                .Stub(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything))
                .Return(Task.Run(() => { }));

            var migrationService = GetMigrationService(versionRepository, new[] { migration });

            await migrationService.MigrateDownPostRollback(MigrationTopic);
            
            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything));

            migration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            migration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            migration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            migration.AssertWasCalled(m => m.MigrateDownPostRollback());
        }

        [TestMethod]
        public async Task MigrateDownPostRollback_Whitelist_ShouldOnlyRollbackIncludedVersions()
        {
            var goodMigration = GetMigration(MigrationTopic, Version);
            var badMigration = GetMigration(MigrationTopic, Version + 1);

            var versionRepository = GetVersionRepository();

            versionRepository
                .Stub(r => r.GetVersionRecords(Arg<string>.Is.Anything))
                .Return(Task.FromResult(new[]
                {
                    VersionRecordFactory.Generate(MigrationTopic, Version, migrateUpPostReleaseRan: null),
                    VersionRecordFactory.Generate(MigrationTopic, Version + 1, migrateUpPostReleaseRan: null)
                }.AsEnumerable()));

            versionRepository
                .Stub(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything))
                .Return(Task.Run(() => { }));

            var migrationService = GetMigrationService(versionRepository, new List<Migration> { goodMigration, badMigration });

            await migrationService.MigrateDownPostRollback(MigrationTopic, new[] { goodMigration.Version });
            
            versionRepository.AssertWasCalled(r => r.InitializeVersionTable());
            versionRepository.AssertWasCalled(r => r.GetVersionRecords(Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.InsertVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Anything, Arg<string>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordComplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasNotCalled(r => r.MarkVersionRecordIncomplete(Arg<string>.Is.Anything, Arg<double>.Is.Anything));
            versionRepository.AssertWasCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Equal(goodMigration.Version)));
            versionRepository.AssertWasNotCalled(r => r.DeleteVersionRecord(Arg<string>.Is.Anything, Arg<double>.Is.Equal(badMigration.Version)));

            goodMigration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            goodMigration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            goodMigration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            goodMigration.AssertWasCalled(m => m.MigrateDownPostRollback());

            badMigration.AssertWasNotCalled(m => m.MigrateUpPreRelease());
            badMigration.AssertWasNotCalled(m => m.MigrateUpPostRelease());
            badMigration.AssertWasNotCalled(m => m.MigrateDownPreRollback());
            badMigration.AssertWasNotCalled(m => m.MigrateDownPostRollback());
        }

        #endregion MigrateDownPostRollback Method

        #region Private Methods

        private static MigrationService GetMigrationService(IVersionRepository versionRepository = null, IEnumerable<Migration> migrations = null)
        {
            if (versionRepository == null) versionRepository = GetVersionRepository();

            return new MigrationService(versionRepository: versionRepository, migrations: migrations ?? new List<Migration>());
        }

        private static IVersionRepository GetVersionRepository()
        {
            var versionRepository = MockRepository.GenerateMock<IVersionRepository>();

            versionRepository
                .Stub(r => r.InitializeVersionTable())
                .Return(Task.Run(() => { }));

            return versionRepository;
        }

        private static Migration GetMigration(string migrationTopic, double version)
        {
            var migration = MockRepository.GenerateMock<Migration>(migrationTopic, version, "Description");

            migration
                .Stub(m => m.MigrateUpPreRelease())
                .Return(Task.Run(() => { Console.WriteLine("Called Migration.MigrateUpPreRelease.\tTopic: {0},\tVersion: {1}", migrationTopic, version); }));

            migration
                .Stub(m => m.MigrateUpPostRelease())
                .Return(Task.Run(() => { Console.WriteLine("Called Migration.MigrateUpPostRelease.\tTopic: {0},\tVersion: {1}", migrationTopic, version); }));

            migration
                .Stub(m => m.MigrateDownPreRollback())
                .Return(Task.Run(() => { Console.WriteLine("Called Migration.MigrateDownPreRollback.\tTopic: {0},\tVersion: {1}", migrationTopic, version); }));

            migration
                .Stub(m => m.MigrateDownPostRollback())
                .Return(Task.Run(() => { Console.WriteLine("Called Migration.MigrateDownPostRollback.\tTopic: {0},\tVersion: {1}", migrationTopic, version); }));

            return migration;
        }

        #endregion Private Methods
    }
}
