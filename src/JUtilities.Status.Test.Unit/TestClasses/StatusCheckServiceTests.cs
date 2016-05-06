using System.Linq;
using System.Threading;
using FluentAssertions;
using JUtilities.Settings;
using JUtilities.Status.Test.Unit.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JUtilities.Status.Test.Unit.TestClasses
{
    [TestClass]
    public class StatusCheckServiceTests
    {
        [TestMethod]
        public void ShouldReturnTrueForValidStatus()
        {
            GetStatusCheckService(new GoodStatusCheck()).GetStatus(typeof(GoodStatusCheck)).IsActive.Should().BeTrue();
        }

        [TestMethod]
        public void ShouldReturnFalseForInvalidStatus()
        {
            GetStatusCheckService(new BadStatusCheck()).GetStatus(typeof(BadStatusCheck)).IsActive.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldReturnExceptionForInvalidStatus()
        {
            GetStatusCheckService(new BadStatusCheck()).GetStatus(typeof(BadStatusCheck)).Exception.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldReturnNullWhenDoesNotExist()
        {
            GetStatusCheckService().GetStatus(typeof(BadStatusCheck)).Should().BeNull();
        }

        [TestMethod]
        public void ShouldReturnAllStatuses()
        {
            GetStatusCheckService(new GoodStatusCheck(), new BadStatusCheck(), new TimeoutStatusCheck()).GetStatuses().Count().ShouldBeEquivalentTo(3);
        }

        [TestMethod]
        public void ShouldReturnTheCorrectStatus()
        {
            var statusCheckService = GetStatusCheckService(new BadStatusCheck(), new GoodStatusCheck());

            statusCheckService.GetStatus(typeof(GoodStatusCheck)).IsActive.Should().BeTrue();
            statusCheckService.GetStatus(typeof(BadStatusCheck)).IsActive.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldReturnFalseWhenTimeoutIsHit()
        {
            GetStatusCheckService(new TimeoutStatusCheck()).GetStatus(typeof(TimeoutStatusCheck)).IsActive.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldUpdateLastUpdatedUtc()
        {
            var statusCheckService = GetStatusCheckService(new GoodStatusCheck());
            var initialLastUpdatedUtc = statusCheckService.LastUpdatedUtc;

            Thread.Sleep(statusCheckService.PollIncrement + 1000);

            statusCheckService.LastUpdatedUtc.Should().BeAfter(initialLastUpdatedUtc);
        }

        [TestMethod]
        public void ShouldUpdateLastUpdatedUtcOnStatus()
        {
            var statusCheckService = GetStatusCheckService(new GoodStatusCheck());
            var initialLastUpdatedUtc = statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc;

            Thread.Sleep(statusCheckService.PollIncrement + 3000);

            statusCheckService.GetStatus(typeof(GoodStatusCheck)).LastUpdatedUtc.Should().BeAfter(initialLastUpdatedUtc);
        }

        [TestMethod]
        public void ShouldReturnFalseWhenDatabaseDoesNotExist()
        {
            var connectionStringProvider = new AppSettingsConnectionStringProvider();

            GetStatusCheckService(new DatabaseStatusCheck(connectionStringProvider)).GetStatus(typeof(DatabaseStatusCheck)).IsActive.Should().BeFalse();
        }

        private StatusCheckService GetStatusCheckService(params IStatusCheck[] statusChecks)
        {
            var settingsProvider = new AppSettingsSettingsProvider();

            return new StatusCheckService(settingsProvider, statusChecks);
        }
    }
}
