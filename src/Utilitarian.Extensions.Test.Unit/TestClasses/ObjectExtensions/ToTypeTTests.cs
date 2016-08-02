using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilitarian.Extensions.Test.Unit.TestClasses.ObjectExtensions
{
    [TestClass]
    public class ToTypeTTests
    {
        [TestMethod]
        public void ShouldCastAsString()
        {
            const string @object = "TestString";

            @object.ToType<string>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsInt()
        {
            const int @object = 5;

            @object.ToType<int>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsIntFromString()
        {
            const int @object = 5;

            @object.ToString().ToType<int>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsEnum()
        {
            const MockEnum @object = MockEnum.Value1;

            @object.ToType<MockEnum>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsEnumFromString()
        {
            const MockEnum @object = MockEnum.Value1;

            @object.ToString().ToType<MockEnum>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsEnumFromInt()
        {
            const MockEnum @object = MockEnum.Value1;

            ((int)@object).ToString().ToType<MockEnum>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsEnumFromIntAsString()
        {
            const MockEnum @object = MockEnum.Value1;

            ((int)@object).ToString().ToType<MockEnum>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsDecimal()
        {
            const decimal @object = 3.14m;

            @object.ToType<decimal>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsDecimalFromString()
        {
            const decimal @object = 3.14m;

            @object.ToString(CultureInfo.InvariantCulture).ToType<decimal>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsDouble()
        {
            const double @object = 3.14D;

            @object.ToType<double>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsDoubleFromString()
        {
            const double @object = 3.14D;

            @object.ToString(CultureInfo.InvariantCulture).ToType<double>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsFloat()
        {
            const float @object = 3.14F;

            @object.ToType<float>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsFloatFromString()
        {
            const string @object = "3.14";

            // ReSharper disable once RedundantToStringCall
            @object.ToString().ToType<float>().ToString(CultureInfo.InvariantCulture).ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsBool()
        {
            const bool @object = true;

            @object.ToType<bool>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsBoolFromString()
        {
            const bool @object = true;

            @object.ToString().ToType<bool>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsGuid()
        {
            var @object = Guid.NewGuid();

            @object.ToType<Guid>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldCastAsGuidFromString()
        {
            var @object = Guid.NewGuid();

            @object.ToString().ToType<Guid>().ShouldBeEquivalentTo(@object);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnInvalidCast()
        {
            const string @object = "TestString";
            Action action = () => @object.ToType<int>();

            action.ShouldThrow<Exception>();
        }

        [TestMethod]
        public void ShouldThrowExceptionIfNull()
        {
            const object @object = null;
            Action action = () => @object.ToType<int>();

            action.ShouldThrow<Exception>();
        }

        [TestMethod]
        public void ShouldThrowExceptionIfNullableType()
        {
            const string @object = "TestString";
            Action action = () => @object.ToType<int?>();

            action.ShouldThrow<Exception>();
        }

        private enum MockEnum
        {
            Value1 = 1,

            // ReSharper disable once UnusedMember.Local
            Value2 = 2
        }
    }
}
