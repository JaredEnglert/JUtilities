using System.Linq;
using System.Threading;
using FluentAssertions;
using Utilitarian.Status.Test.Unit.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilitarian.Settings.Test.Unit.Mocks;

namespace Utilitarian.Status.Test.Unit.TestClasses
{
    [TestClass]
    public class StatusCheckServiceTests
    {
        private readonly MockSettingsProvider _mockSettingsProvider;

        private readonly MockConnectionStringProvider _mockConnectionStringProvider;

        public StatusCheckServiceTests()
        {
            _mockSettingsProvider = new MockSettingsProvider();
            _mockSettingsProvider.Settings.Add("StatusCheck:TimeoutLimitInSeconds", 1);
            _mockSettingsProvider.Settings.Add("StatusCheck:PollIncrementInSeconds", 3);

            _mockConnectionStringProvider = new MockConnectionStringProvider();
            _mockConnectionStringProvider.ConnectionStrings.Add("TestConnectionString", "Server=TestServer;Database=TestDatabase;User Id=TestUser;Password=password;");
        }

        [TestMethod]
        public void ShouldReturnTrueForValidStatus()
        {
            var statusCheckService = CreateStatusCheckService();

            statusCheckService.GetStatus(typeof(GoodStatusCheck)).IsActive.Should().BeTrue();
        }

        [TestMethod]
        public void ShouldReturnFalseForInvalidStatus()
        {
            var statusCheckService = CreateStatusCheckService();

            statusCheckService.GetStatus(typeof(BadStatusCheck)).IsActive.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldReturnExceptionForInvalidStatus()
        {
            var statusCheckService = CreateStatusCheckService();

            statusCheckService.GetStatus(typeof(BadStatusCheck)).Exception.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldReturnNullWhenDoesNotExist()
        {
            var statusCheckService = CreateStatusCheckService();

            statusCheckService.GetStatus(typeof(UnusedStatusCheck)).Should().BeNull();
        }

        [TestMethod]
        public void ShouldReturnAllStatuses()
        {
            var statusCheckService = CreateStatusCheckService();

            statusCheckService.GetStatuses().Count().ShouldBeEquivalentTo(4);
        }

        [TestMethod]
        public void ShouldReturnTheCorrectStatus()
        {
            var statusCheckService = CreateStatusCheckService();

            statusCheckService.GetStatus(typeof(GoodStatusCheck)).IsActive.Should().BeTrue();
            statusCheckService.GetStatus(typeof(BadStatusCheck)).IsActive.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldReturnFalseWhenTimeoutIsHit()
        {
            var statusCheckService = CreateStatusCheckService();

            statusCheckService.GetStatus(typeof(TimeoutStatusCheck)).IsActive.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldUpdateLastUpdatedUtc()
        {
            var statusCheckService = CreateStatusCheckService();
            var initialLastUpdatedUtc = statusCheckService.LastUpdatedUtc;

            statusCheckService.ForceUpdate();

            statusCheckService.LastUpdatedUtc.Should().BeAfter(initialLastUpdatedUtc);
        }

        [TestMethod]
        public void ShouldUpdateLastUpdatedUtcOnStatus()
        {
            var statusCheckService = CreateStatusCheckService();
            var initialLastUpdatedUtc = statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc;

            statusCheckService.ForceUpdate();

            statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc.Should().BeAfter(initialLastUpdatedUtc);
        }

        [TestMethod]
        public void ShouldContinueToPollAfterForceUpdate()
        {
            var statusCheckService = CreateStatusCheckService();
            var initialLastUpdatedUtc = statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc;

            statusCheckService.ForceUpdate();
            
            initialLastUpdatedUtc = statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc;

            Thread.Sleep(statusCheckService.PollIncrement + statusCheckService.TimeoutLimit + 6000);

            statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc.Should().BeAfter(initialLastUpdatedUtc);
        }

        [TestMethod]
        public void ShouldReturnFalseWhenDatabaseDoesNotExist()
        {
            var statusCheckService = CreateStatusCheckService();

            statusCheckService.GetStatus(typeof(DatabaseStatusCheck)).IsActive.Should().BeFalse();
        }

        private StatusCheckService CreateStatusCheckService()
        {
            return new StatusCheckService(_mockSettingsProvider, new IStatusCheck[] {
                new GoodStatusCheck(),
                new BadStatusCheck(),
                new TimeoutStatusCheck(_mockSettingsProvider),
                new DatabaseStatusCheck(_mockConnectionStringProvider)
            });
        }
    }
}
