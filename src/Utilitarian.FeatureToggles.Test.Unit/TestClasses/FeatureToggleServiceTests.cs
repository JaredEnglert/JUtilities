using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilitarian.FeatureToggles.Test.Unit.TestClasses
{
    [TestClass]
    public class FeatureToggleServiceTests
    {
        private readonly IFeatureToggleService _featureToggleService;

        public FeatureToggleServiceTests()
        {
            _featureToggleService = new FeatureToggleService(new List<IFeatureToggle> {
                new TrueFeature(),
                new FalseFeature(),
                new DynamicFeature()
            });
        }

        [TestMethod]
        public void ShouldReturnTrueWhenFeatureIsOn()
        {
            _featureToggleService.IsEnabled("TrueFeature").ShouldBeEquivalentTo(true);
        }

        [TestMethod]
        public void ShouldReturnFalseWhenFeatureIsOff()
        {
            _featureToggleService.IsEnabled("FalseFeature").ShouldBeEquivalentTo(false);
        }

        [TestMethod]
        public void ShouldReturnFalseWhenFeatureDoesNotExist()
        {
            _featureToggleService.IsEnabled("DoesNotExist").ShouldBeEquivalentTo(false);
        }

        [TestMethod]
        public void ShouldReturnFalseWhenNameIsNull()
        {
            _featureToggleService.IsEnabled(null).ShouldBeEquivalentTo(false);
        }

        [TestMethod]
        public void ShouldReturnCorrectDynamicValue()
        {
            _featureToggleService.IsEnabled("DynamicFeature", true).ShouldBeEquivalentTo(false);
        }

        private class TrueFeature : IFeatureToggle
        {
            public string Name => "TrueFeature";

            public bool IsEnabled(object @object = null)
            {
                return true;
            }
        }

        private class FalseFeature : IFeatureToggle
        {
            public string Name => "FalseFeature";

            public bool IsEnabled(object @object = null)
            {
                return false;
            }
        }

        private class DynamicFeature : IFeatureToggle
        {
            public string Name => "DynamicFeature";

            public bool IsEnabled(object @object = null)
            {
                return !(bool)@object;
            }
        }
    }
}
