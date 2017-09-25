using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilitarian.Migrations.Repositories.MongoDb;
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
            versionRepository.Should().BeOfType<MongoDbVersionRepository>();
        }

        [TestMethod]
        public void GetVersionRepository_WrongCase_ShouldReturnCorrectImplementation()
        {
            var versionService = GetVersionService();

            var versionRepository = versionService.GetVersionRepository("mOnGoDb", DatabaseName, ConnectionString);

            versionRepository.Should().NotBeNull();
            versionRepository.Should().BeOfType<MongoDbVersionRepository>();
        }

        [TestMethod]
        public void GetVersionRepository_MissingImplementation_ShouldThrowException()
        {
            var versionService = GetVersionService();
            
            Action action = () => versionService.GetVersionRepository("Bad", DatabaseName, ConnectionString);

            action.ShouldThrow<Exception>();
        }

        #endregion GetVersionRepository Method

        #region Private Methods

        private static RepositoryService GetVersionService()
        {
            return new RepositoryService();
        }

        #endregion Private Methods
    }
}
