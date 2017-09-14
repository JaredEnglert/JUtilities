using System;
using System.Security.Cryptography;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilitarian.Extensions.Test.Unit.TestClasses.ObjectExtensions
{
    [TestClass]
    public class GetHashTests
    {
        [TestMethod]
        public void GetHash_ShouldHash()
        {
            var hash = 2.GetHash<MD5CryptoServiceProvider>();

            hash.Should().NotBeNullOrWhiteSpace();
            hash.Should().BeOfType<string>();
        }

        [TestMethod]
        public void GetHash_EquivalentObjects_ShouldHashTheSame()
        {
            new TestClass(1).GetHash<MD5CryptoServiceProvider>().Should().Be(new TestClass(1).GetHash<MD5CryptoServiceProvider>());
        }

        [TestMethod]
        public void GetHash_DifferentObjects_ShouldHashDifferently()
        {
            new TestClass(1).GetHash<MD5CryptoServiceProvider>().Should().NotBe(new TestClass(2).GetHash<MD5CryptoServiceProvider>());
        }

        [TestMethod]
        public void GetHash_ChangesToSubObject_ShouldHashDifferently()
        {
            var array = new[]
            {
                new TestClass(1),
                new TestClass(2)
            };

            var originalHash = array.GetHash<MD5CryptoServiceProvider>();

            array[0].Value = 4;

            array.GetHash<MD5CryptoServiceProvider>().Should().NotBe(originalHash);
        }

        [Serializable]
        private class TestClass
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int Value { get; set; }

            public TestClass(int value)
            {
                Value = value;
            }
        }
    }
}
