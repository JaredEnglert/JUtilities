using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Utilitarian.Extensions.Test.Unit.TestClasses.CollectionExtensions
{
    [TestClass]
    public class RemoveItemTests
    {
        [TestMethod]
        public void NoMatches_ShouldNotRemoveAnyItems()
        {
            var collection = GetCollection();

            var count = collection.Count;

            collection.RemoveItem(x => x == -1);

            collection.Count.Should().Be(count);
        }

        [TestMethod]
        public void OneMatches_ShouldRemoveItem()
        {
            var collection = GetCollection();
            var count = collection.Count;

            collection.RemoveItem(x => x == 1);

            collection.Count.Should().Be(count - 1);
        }

        [TestMethod]
        public void MultipleMatches_ShouldRemoveOnlyOneItem()
        {
            var collection = GetCollection();
            collection.Add(123);
            collection.Add(123);
            collection.Add(123);

            var count = collection.Count;

            collection.RemoveItem(x => x == 123);

            collection.Count.Should().Be(count - 1);
        }

        private static ICollection<int>  GetCollection()
        {
            return new List<int>
            {
                1,
                2,
                3,
                4
            };
        }
    }
}
