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
            typeof(TestClass).ImplementsInterface<ITestInterface>().Should().BeTrue();
        }

        [TestMethod]
        public void ShouldBeFalseWhenNotImplemented()
        {
            typeof(int).ImplementsInterface<ITestInterface>().Should().BeFalse();
        }

        [TestMethod]
        public void ShouldThowExceptionWhenTIsNotAnInterface()
        {
            Action action = () => typeof(int).ImplementsInterface<int>();

            action.ShouldThrow<Exception>();
        }

        private interface ITestInterface
        {

        }

        private class TestClass : ITestInterface
        {

        }
    }
}
