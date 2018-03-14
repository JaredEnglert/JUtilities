using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Utilitarian.Migrations.Interfaces;
using Utilitarian.Migrations.Services;

namespace Utilitarian.Migrations.Test.Unit.Tests.Services
{
    [TestClass]
    public class VersionServiceTests
    {
        private const string DatabaseType = "MONGODB";

        private const string DatabaseName = "DatabaseName";

        private const string ConnectionString = "ConnectionString";

        #region GetVersionRepository Method

        [TestMethod]
        public void GetVersionRepository_NoDatabaseType_ShouldThrowException()
        {
            var versionService = GetVersionService();

            Action actionNull = () => versionService.GetVersionRepository(null, DatabaseName, ConnectionString);
            Action actionBlank = () => versionService.GetVersionRepository("  ", DatabaseName, ConnectionString);

            actionNull.ShouldThrow<Exception>();
            actionBlank.ShouldThrow<Exception>();
        }

        [TestMethod]
        public void GetVersionRepository_NoDatabaseName_ShouldThrowException()
        {
            var versionService = GetVersionService();

            Action actionNull = () => versionService.GetVersionRepository(DatabaseType, null, ConnectionString);
            Action actionBlank = () => versionService.GetVersionRepository(DatabaseType, "  ", ConnectionString);

            actionNull.ShouldThrow<Exception>();
            actionBlank.ShouldThrow<Exception>();
        }

        [TestMethod]
        public void GetVersionRepository_NoConnectionString_ShouldThrowException()
        {
            var versionService = GetVersionService();

            Action actionNull = () => versionService.GetVersionRepository(DatabaseType, DatabaseName, null);
            Action actionBlank = () => versionService.GetVersionRepository(DatabaseType, DatabaseName, "  ");

            actionNull.ShouldThrow<Exception>();
            actionBlank.ShouldThrow<Exception>();
        }

        [TestMethod]
        public void GetVersionRepository_ExistingImplementation_ShouldReturnCorrectImplementation()
        {
            var versionService = GetVersionService();

            var versionRepository = versionService.GetVersionRepository(DatabaseType, DatabaseName, ConnectionString);

            versionRepository.Should().NotBeNull();
            versionRepository.DatabaseType.Should().Be(DatabaseType);
        }

        [TestMethod]
        public void GetVersionRepository_WrongCase_ShouldReturnCorrectImplementation()
        {
            var versionService = GetVersionService();

            var versionRepository = versionService.GetVersionRepository("mOnGoDb", DatabaseName, ConnectionString);

            versionRepository.Should().NotBeNull();
            versionRepository.DatabaseType.Should().Be(DatabaseType);
        }

        [TestMethod]
        public void GetVersionRepository_MissingImplementation_ShouldReturnNull()
        {
            var versionService = GetVersionService();

            var versionRepository = versionService.GetVersionRepository("Bad", DatabaseName, ConnectionString);

            versionRepository.Should().BeNull();
        }

        #endregion GetVersionRepository Method

        #region GetDatabaseUtilityRepository Method

        [TestMethod]
        public void GetDatabaseUtilityRepository_NoDatabaseType_ShouldThrowException()
        {
            var versionService = GetVersionService();

            Action actionNull = () => versionService.GetDatabaseUtilityRepository(null, DatabaseName, ConnectionString);
            Action actionBlank = () => versionService.GetDatabaseUtilityRepository("  ", DatabaseName, ConnectionString);

            actionNull.ShouldThrow<Exception>();
            actionBlank.ShouldThrow<Exception>();
        }

        [TestMethod]
        public void GetDatabaseUtilityRepository_NoDatabaseName_ShouldThrowException()
        {
            var versionService = GetVersionService();

            Action actionNull = () => versionService.GetDatabaseUtilityRepository(DatabaseType, null, ConnectionString);
            Action actionBlank = () => versionService.GetDatabaseUtilityRepository(DatabaseType, "  ", ConnectionString);

            actionNull.ShouldThrow<Exception>();
            actionBlank.ShouldThrow<Exception>();
        }

        [TestMethod]
        public void GetDatabaseUtilityRepository_NoConnectionString_ShouldThrowException()
        {
            var versionService = GetVersionService();

            Action actionNull = () => versionService.GetDatabaseUtilityRepository(DatabaseType, DatabaseName, null);
            Action actionBlank = () => versionService.GetDatabaseUtilityRepository(DatabaseType, DatabaseName, "  ");

            actionNull.ShouldThrow<Exception>();
            actionBlank.ShouldThrow<Exception>();
        }

        [TestMethod]
        public void GetDatabaseUtilityRepository_ExistingImplementation_ShouldReturnCorrectImplementation()
        {
            var versionService = GetVersionService();

            var versionRepository = versionService.GetDatabaseUtilityRepository(DatabaseType, DatabaseName, ConnectionString);

            versionRepository.Should().NotBeNull();
            versionRepository.DatabaseType.Should().Be(DatabaseType);
        }

        [TestMethod]
        public void GetDatabaseUtilityRepository_WrongCase_ShouldReturnCorrectImplementation()
        {
            var versionService = GetVersionService();

            var versionRepository = versionService.GetDatabaseUtilityRepository("mOnGoDb", DatabaseName, ConnectionString);

            versionRepository.Should().NotBeNull();
            versionRepository.DatabaseType.Should().Be(DatabaseType);
        }

        [TestMethod]
        public void GetDatabaseUtilityRepository_MissingImplementation_ShouldReturnNull()
        {
            var versionService = GetVersionService();

            var versionRepository = versionService.GetDatabaseUtilityRepository("Bad", DatabaseName, ConnectionString);

            versionRepository.Should().BeNull();
        }

        #endregion GetDatabaseUtilityRepository Method

        #region Private Methods

        private static RepositoryService GetVersionService()
        {
            return new RepositoryService(GetVersionRepositories(), GetDatabaseUtilityServices());
        }

        private static IEnumerable<IVersionRepository> GetVersionRepositories()
        {
            var versionRepository = MockRepository.GenerateMock<IVersionRepository>();
            var otherVersionRepository = MockRepository.GenerateMock<IVersionRepository>();

            versionRepository.Stub(r => r.DatabaseType).Return(DatabaseType);
            otherVersionRepository.Stub(r => r.DatabaseType).Return("Other");

            return new[]
            {
                versionRepository,
                otherVersionRepository
            };
        }

        private static IEnumerable<IDatabaseUtilityService> GetDatabaseUtilityServices()
        {
            var databaseUtilityService = MockRepository.GenerateMock<IDatabaseUtilityService>();
            var otherDatabaseUtilityService = MockRepository.GenerateMock<IDatabaseUtilityService>();

            databaseUtilityService.Stub(r => r.DatabaseType).Return(DatabaseType);
            otherDatabaseUtilityService.Stub(r => r.DatabaseType).Return("Other");

            return new[]
            {
                databaseUtilityService,
                otherDatabaseUtilityService
            };
        }

        #endregion Private Methods
    }
}
