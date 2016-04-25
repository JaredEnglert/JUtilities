﻿using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JUtilities.Settings.Test.Unit.TestClasses
{
    [TestClass]
    public class AppSettingsSettingsProviderTests
    {
        [TestMethod]
        public void ShouldReturnValueWhenSettingExists()
        {
            new AppSettingsSettingsProvider().Get<int>("TestInt").ShouldBeEquivalentTo(1);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenSettingDoesNotExist()
        {
            try
            {
                new AppSettingsSettingsProvider().Get<int>("BadKey");

                Assert.Fail();
            }
            catch (Exception exception)
            {
                exception.Should().NotBeNull();
            }
        }

        [TestMethod]
        public void ShouldReturnTrueWhenSettingExists()
        {
            new AppSettingsSettingsProvider().Contains("TestInt").Should().BeTrue();
        }

        [TestMethod]
        public void ShouldReturnFalseWhenSettingDoesNotExist()
        {
            new AppSettingsSettingsProvider().Contains("BadKey").Should().BeFalse();
        }

        [TestMethod]
        public void ShouldReturnTrueAndValueWhenSettingExists()
        {
            int testInt;
            new AppSettingsSettingsProvider().TryGet("TestInt", out testInt).Should().BeTrue();

            testInt.ShouldBeEquivalentTo(1);
        }

        [TestMethod]
        public void ShouldReturnFalseAndDefaultValueWhenSettingDoesNotExist()
        {
            int testInt;
            new AppSettingsSettingsProvider().TryGet("BadKey", out testInt).Should().BeFalse();

            testInt.ShouldBeEquivalentTo(default(int));
        }
    }
}
