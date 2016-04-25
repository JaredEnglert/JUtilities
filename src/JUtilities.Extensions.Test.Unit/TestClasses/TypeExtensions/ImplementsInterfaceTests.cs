using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JUtilities.Extensions.Test.Unit.TestClasses.TypeExtensions
{
    [TestClass]
    public class ImplementsInterfaceTests
    {
        [TestMethod]
        public void ShouldBeTrueWhenImplemented()
        {
            typeof(TestClass).ImplementsInterface<TestInterface>().Should().BeTrue();
        }

        [TestMethod]
        public void ShouldBeFalseWhenNotImplemented()
        {
            typeof(int).ImplementsInterface<TestInterface>().Should().BeFalse();
        }

        [TestMethod]
        public void ShouldThowExceptionWhenTIsNotAnInterface()
        {
            try
            {
                typeof(int).ImplementsInterface<int>();

                Assert.Fail();
            }
            catch (Exception exception)
            {
                exception.Should().NotBeNull();
            }
        }

        private interface TestInterface
        {

        }

        private class TestClass : TestInterface
        {

        }
    }
}
