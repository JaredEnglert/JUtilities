using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilitarian.Extensions.Test.Unit.TestClasses.TypeExtensions
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
            Action action = () => typeof(int).ImplementsInterface<int>();

            action.ShouldThrow<Exception>();
        }

        private interface TestInterface
        {

        }

        private class TestClass : TestInterface
        {

        }
    }
}
