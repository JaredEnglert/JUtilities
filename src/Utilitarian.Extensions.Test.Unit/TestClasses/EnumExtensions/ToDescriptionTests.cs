using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilitarian.Extensions.Test.Unit.TestClasses.EnumExtensions
{
    [TestClass]
    public class ToDescriptionTests
    {
        private const string Description = "Test Description";

        [TestMethod]
        public void ShouldReturnDescriptionAttributeWhenExists()
        {
            TestEnum.Description.ToDescription().ShouldBeEquivalentTo(Description);
        }

        [TestMethod]
        public void ShouldReturnEnumToStringWhenNoDescriptionAttributeExists()
        {
            TestEnum.NoDescription.ToDescription().ShouldBeEquivalentTo(TestEnum.NoDescription.ToString());
        }

        private enum TestEnum
        {
            [System.ComponentModel.Description(ToDescriptionTests.Description)]
            Description = 1,

            NoDescription = 2
        }
    }
}
