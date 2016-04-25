using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JUtilities.Settings.Test.Unit.TestClasses
{
    [TestClass]
    public class AppSettingsApplicationProviderTests
    {
        [TestMethod]
        public void ShouldBeTheCorrectApplicationName()
        {
            new AppSettingsApplicationProvider().ApplicationName.ShouldBeEquivalentTo("JUtilities Settings Unit Test");
        }
    }
}
