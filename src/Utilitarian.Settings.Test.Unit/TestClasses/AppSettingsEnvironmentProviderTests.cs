﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilitarian.Settings.Test.Unit.TestClasses
{
    [TestClass]
    public class AppSettingsEnvironmentProviderTests
    {
        [TestMethod]
        public void ShouldBeTheCorrectEnvironmentName()
        {
            new AppSettingsEnvironmentProvider().EnvironmentName.ShouldBeEquivalentTo("UnitTest");
        }
    }
}
