using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilitarian.Assemblies.Test.Unit.Models;

namespace Utilitarian.Assemblies.Test.Unit.Tests
{
    [TestClass]
    public class AssemblyServiceTests
    {
        #region Get Method

        [TestMethod]
        public void Get_ShouldGetAllImplementations()
        {
            var assemblyService = GetAssemblyService();

            var testBases = assemblyService.GetAllImplementations<TestBase>().ToList();

            testBases.Should().NotBeNull();
            testBases.Should().BeOfType<List<TestBase>>();
            testBases.Count.Should().Be(2);
            testBases.Any(testBase => testBase.GetType() == typeof(TestInherited)).Should().BeTrue();
            testBases.Any(testBase => testBase.GetType() == typeof(TestTopLevelInheritence)).Should().BeTrue();
            testBases.Sum(testBase => testBase.TestMethod()).Should().Be(3);
        }

        #endregion Method

        #region Private Methods

        private static AssemblyService GetAssemblyService()
        {
            return new AssemblyService();
        }

        #endregion Private Methods
    }
}
