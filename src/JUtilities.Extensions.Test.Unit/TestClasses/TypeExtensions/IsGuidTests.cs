using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JUtilities.Extensions.Test.Unit.TestClasses.TypeExtensions
{
    [TestClass]
    public class IsGuidTests
    {
        [TestMethod]
        public void ShouldBeTrueForGuid()
        {
            Guid.NewGuid().GetType().IsGuid().Should().BeTrue();
        }

        [TestMethod]
        public void ShouldBeFalseForNonGuid()
        {
            1.GetType().IsGuid().Should().BeFalse();
        }

        [TestMethod]
        public void ShouldBeTrueForNullableGuid()
        {
            Guid? guid = Guid.NewGuid();

            guid.GetType().IsGuid().Should().BeTrue();
        }
    }
}
