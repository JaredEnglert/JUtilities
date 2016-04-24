using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JUtilities.Extensions.Test.Unit.Mocks;

namespace JUtilities.Extensions.Test.Unit.TestClasses.ObjectExtensions
{
    [TestClass]
    public class ToTypeTTests
    {
        [TestMethod]
        public void ItShouldCastAsString()
        {
            const string @object = "TestString";

            @object.ToType<string>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsInt()
        {
            const int @object = 5;

            @object.ToType<int>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsIntFromString()
        {
            const int @object = 5;

            @object.ToString().ToType<int>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsEnum()
        {
            const MockEnum @object = MockEnum.Value1;

            @object.ToType<MockEnum>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsEnumFromString()
        {
            const MockEnum @object = MockEnum.Value1;

            @object.ToString().ToType<MockEnum>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsEnumFromInt()
        {
            const MockEnum @object = MockEnum.Value1;

            ((int)@object).ToString().ToType<MockEnum>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsEnumFromIntAsString()
        {
            const MockEnum @object = MockEnum.Value1;

            ((int)@object).ToString().ToType<MockEnum>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsDecimal()
        {
            const decimal @object = 3.14m;

            @object.ToType<decimal>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsDecimalFromString()
        {
            const decimal @object = 3.14m;

            @object.ToString().ToType<decimal>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsDouble()
        {
            const double @object = 3.14D;

            @object.ToType<double>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsDoubleFromString()
        {
            const double @object = 3.14D;

            @object.ToString().ToType<double>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsFloat()
        {
            const float @object = 3.14F;

            @object.ToType<float>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsFloatFromString()
        {
            const string @object = "3.14";

            @object.ToString().ToType<float>().ToString().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsBool()
        {
            const bool @object = true;

            @object.ToType<bool>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsBoolFromString()
        {
            const bool @object = true;

            @object.ToString().ToType<bool>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsGuid()
        {
            var @object = Guid.NewGuid();

            @object.ToType<Guid>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldCastAsGuidFromString()
        {
            var @object = Guid.NewGuid();

            @object.ToString().ToType<Guid>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ItShouldThrowExceptionOnInvalidCast()
        {
            const string @object = "TestString";

            try
            {
                @object.ToType<int>();

                Assert.Fail();
            }
            catch (Exception exception)
            {
                exception.Should().NotBeNull();
            }
        }

        [TestMethod]
        public void ItShouldThrowExceptionIfNull()
        {
            const object @object = null;

            try
            {
                @object.ToType<int>();

                Assert.Fail();
            }
            catch (Exception exception)
            {
                exception.Should().NotBeNull();
            }
        }

        [TestMethod]
        public void ItShouldThrowExceptionIfNullableType()
        {
            const string @object = "TestString";

            try
            {
                @object.ToType<int?>();

                Assert.Fail();
            }
            catch (Exception exception)
            {
                exception.Should().NotBeNull();
            }
        }
    }
}
