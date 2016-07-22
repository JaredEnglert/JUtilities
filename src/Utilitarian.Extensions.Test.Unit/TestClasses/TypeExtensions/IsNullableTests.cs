using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilitarian.Extensions.Test.Unit.TestClasses.TypeExtensions
{
    [TestClass]
    public class IsNullableTests
    {
        [TestMethod]
        public void ShouldBeTrueWhenNullable()
        {
            (typeof (DateTime?)).IsNullable().Should().BeTrue();
        }

        [TestMethod]
        public void ShouldBeFalseWhenNotNullable()
        {
            (typeof(DateTime)).GetType().IsNullable().Should().BeFalse();
        }
    }
}
