using System.Linq;
using System.Threading;
using FluentAssertions;
using Utilitarian.Settings;
using Utilitarian.Status.Test.Unit.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilitarian.Settings.Test.Unit.Mocks;

namespace Utilitarian.Status.Test.Unit.TestClasses
{
    [TestClass]
    public class StatusCheckServiceTests
    {
        private readonly StatusCheckService _statusCheckService;

        public StatusCheckServiceTests()
        {
            var settingsProvider = new MockSettingsProvider();
            settingsProvider.Settings.Add("StatusCheck:TimeoutLimitInSeconds", 2);
            settingsProvider.Settings.Add("StatusCheck:PollIncrementInSeconds", 5);

            var connectionStringProvider = new MockConnectionStringProvider();
            connectionStringProvider.ConnectionStrings.Add("TestConnectionString", "Server=TestServer;Database=TestDatabase;User Id=TestUser;Password=password;");

            _statusCheckService = new StatusCheckService(settingsProvider, new IStatusCheck[] {
                new GoodStatusCheck(),
                new BadStatusCheck(),
                new TimeoutStatusCheck(),
                new DatabaseStatusCheck(connectionStringProvider)
            });
        }

        [TestMethod]
        public void ShouldReturnTrueForValidStatus()
        {
            _statusCheckService.GetStatus(typeof(GoodStatusCheck)).IsActive.Should().BeTrue();
        }

        [TestMethod]
        public void ShouldReturnFalseForInvalidStatus()
        {
            _statusCheckService.GetStatus(typeof(BadStatusCheck)).IsActive.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldReturnExceptionForInvalidStatus()
        {
            _statusCheckService.GetStatus(typeof(BadStatusCheck)).Exception.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldReturnNullWhenDoesNotExist()
        {
            _statusCheckService.GetStatus(typeof(UnusedStatusCheck)).Should().BeNull();
        }

        [TestMethod]
        public void ShouldReturnAllStatuses()
        {
            _statusCheckService.GetStatuses().Count().ShouldBeEquivalentTo(4);
        }

        [TestMethod]
        public void ShouldReturnTheCorrectStatus()
        {
            _statusCheckService.GetStatus(typeof(GoodStatusCheck)).IsActive.Should().BeTrue();
            _statusCheckService.GetStatus(typeof(BadStatusCheck)).IsActive.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldReturnFalseWhenTimeoutIsHit()
        {
            _statusCheckService.GetStatus(typeof(TimeoutStatusCheck)).IsActive.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldUpdateLastUpdatedUtc()
        {
            var initialLastUpdatedUtc = _statusCheckService.LastUpdatedUtc;

            _statusCheckService.ForceUpdate();

            _statusCheckService.LastUpdatedUtc.Should().BeAfter(initialLastUpdatedUtc);
        }

        [TestMethod]
        public void ShouldUpdateLastUpdatedUtcOnStatus()
        {
            var initialLastUpdatedUtc = _statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc;

            _statusCheckService.ForceUpdate();

            _statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc.Should().BeAfter(initialLastUpdatedUtc);
        }

        [TestMethod]
        public void ShouldContinueToPollAfterForceUpdate()
        {
            var initialLastUpdatedUtc = _statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc;

            _statusCheckService.ForceUpdate();

            initialLastUpdatedUtc = _statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc;
            _statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc.Should().BeAfter(initialLastUpdatedUtc);

            initialLastUpdatedUtc = _statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc;

            Thread.Sleep(_statusCheckService.PollIncrement + _statusCheckService.TimeoutLimit + 6000);

            _statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc.Should().BeAfter(initialLastUpdatedUtc);
        }

        [TestMethod]
        public void ShouldReturnFalseWhenDatabaseDoesNotExist()
        {
            _statusCheckService.GetStatus(typeof(DatabaseStatusCheck)).IsActive.Should().BeFalse();
        }
    }
}
