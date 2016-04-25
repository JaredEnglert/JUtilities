using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JUtilities.Settings.Test.Unit.TestClasses
{
    [TestClass]
    public class AppSettingsConnectionStringProviderTests
    {
        [TestMethod]
        public void ShouldReturnCorrectConnectionString()
        {
            new AppSettingsConnectionStringProvider().Get("TestConnectionString").ShouldBeEquivalentTo("TestConnectionString");
        }

        [TestMethod]
        public void ShouldThrowExceptionIfConnectionStringIsMissing()
        {
            try
            {
                new AppSettingsConnectionStringProvider().Get("BadTestConnectionString");

                Assert.Fail();
            }
            catch (Exception exception)
            {
                exception.Should().NotBeNull();
            }
        }
    }
}
