using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Utilitarian.Status.Test.Unit.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Utilitarian.Status.Test.Unit.TestClasses
{
    [TestClass]
    public class StatusCheckServiceTests
    {
        private IStatusCheck goodStatusCheck;

        private IStatusCheck badStatusCheck;

        private IStatusCheck timeoutStatusCheck;

        private IStatusCheck databaseStatusCheck;

        [TestInitialize]
        public void TestInitialize()
        {
            goodStatusCheck = new GoodStatusCheck();
            badStatusCheck = new BadStatusCheck();
            timeoutStatusCheck = new TimeoutStatusCheck();
            databaseStatusCheck = new DatabaseStatusCheck(GetConnectionStringProvider());
        }

        #region GetStatus Method

        [TestMethod]
        public void GetStatus_DoesNotExist_ShouldReturnNull()
        {
            GetStatusCheckService(new IStatusCheck[0]).GetStatus(typeof(UnusedStatusCheck)).Should().BeNull();
        }

        [TestMethod]
        public void GetStatus_DoesExist_ShouldReturnStatus()
        {
            GetStatusCheckService(new[] { goodStatusCheck }).GetStatus(typeof(GoodStatusCheck)).Should().NotBeNull();
        }

        [TestMethod]
        public void GetStatus_ValidStatus_ShouldReturnTrue()
        {
            GetStatusCheckService(new [] { goodStatusCheck }).GetStatus(typeof(GoodStatusCheck)).IsActive.Should().BeTrue();
        }

        [TestMethod]
        public void GetStatus_InvalidStatus_ShouldReturnFalse()
        {
            GetStatusCheckService(new[] { badStatusCheck }).GetStatus(typeof(BadStatusCheck)).IsActive.Should().BeFalse();
        }

        [TestMethod]
        public void GetStatus_InvalidStatus_ShouldReturnException()
        {
            GetStatusCheckService(new[] { badStatusCheck }).GetStatus(typeof(BadStatusCheck)).Exception.Should().NotBeNull();
        }

        [TestMethod]
        public void GetStatus_TimeoutIsHit_ShouldReturnFalse()
        {
            GetStatusCheckService(new[] { timeoutStatusCheck }).GetStatus(typeof(TimeoutStatusCheck)).IsActive.Should().BeFalse();
        }

        [TestMethod]
        public void GetStatus_DatabaseDoesNotExist_ShouldReturnFalse()
        {
            GetStatusCheckService(new[] { databaseStatusCheck }).GetStatus(typeof(DatabaseStatusCheck)).IsActive.Should().BeFalse();
        }

        #endregion GetStatus Method

        #region GetStatuses Method

        [TestMethod]
        public void GetStatuses_ShouldReturnAllStatuses()
        {
            GetStatusCheckService().GetStatuses().Count().ShouldBeEquivalentTo(4);
        }

        #endregion GetStatuses Method

        #region ForceUpdate Method

        [TestMethod]
        public void ForceUpdate_ShouldUpdateLastUpdatedUtc()
        {
            var statusCheckService = GetStatusCheckService(new[] { goodStatusCheck });
            var initialLastUpdatedUtc = statusCheckService.LastUpdatedUtc;

            statusCheckService.ForceUpdate();

            statusCheckService.LastUpdatedUtc.Should().BeAfter(initialLastUpdatedUtc);
        }

        [TestMethod]
        public void ForceUpdate_ShouldUpdateLastUpdatedUtcOnStatus()
        {
            var statusCheckService = GetStatusCheckService(new[] { goodStatusCheck });
            var initialLastUpdatedUtc = statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc;

            statusCheckService.ForceUpdate();

            statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc.Should().BeAfter(initialLastUpdatedUtc);
        }

        [TestMethod]
        public async Task ForceUpdate_ShouldContinueToPollAfterForceUpdate()
        {
            var statusCheckService = GetStatusCheckService(new[] { goodStatusCheck });
            statusCheckService.GetStatus(typeof(GoodStatusCheck));

            statusCheckService.ForceUpdate();

            var initialLastUpdatedUtc = statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc;

            await Task.Delay(statusCheckService.PollIncrement + statusCheckService.TimeoutLimit + 6000);

            statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc.Should().BeAfter(initialLastUpdatedUtc);
        }

        #endregion ForceUpdate Method

        #region Private Methods

        private StatusCheckService GetStatusCheckService(IEnumerable<IStatusCheck> statusChecks = null, ISettingsProvider settingsProvider = null)
        {
            if (settingsProvider == null) settingsProvider = GetSettingsProvider();

            return new StatusCheckService(settingsProvider, statusChecks ?? new[]
            {
                goodStatusCheck,
                badStatusCheck,
                timeoutStatusCheck,
                databaseStatusCheck
            });
        }

        private static ISettingsProvider GetSettingsProvider()
        {
            var settingsProvider = MockRepository.GenerateMock<ISettingsProvider>();

            settingsProvider.Stub(p => p.Get("StatusCheck:TimeoutLimitInSeconds")).Return(2);
            settingsProvider.Stub(p => p.Get("StatusCheck:PollIncrementInSeconds")).Return(5);

            return settingsProvider;
        }

        private static IConnectionStringProvider GetConnectionStringProvider()
        {
            var connectionStringProvider = MockRepository.GenerateMock<IConnectionStringProvider>();

            connectionStringProvider.Stub(p => p.Get("TestConnectionString")).Return("Server=TestServer;Database=TestDatabase;User Id=TestUser;Password=password;");

            return connectionStringProvider;
        }

        #endregion Private Methods
    }
}
