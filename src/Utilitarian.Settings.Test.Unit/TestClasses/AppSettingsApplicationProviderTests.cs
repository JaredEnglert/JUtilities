using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilitarian.Settings.Test.Unit.TestClasses
{
    [TestClass]
    public class AppSettingsApplicationProviderTests
    {
        [TestMethod]
        public void ShouldBeTheCorrectApplicationName()
        {
            new AppSettingsApplicationProvider().ApplicationName.ShouldBeEquivalentTo("Utilitarian Settings Unit Test");
        }
    }
}
