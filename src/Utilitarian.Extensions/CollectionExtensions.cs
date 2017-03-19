using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilitarian.Extensions
{
    public static class CollectionExtensions
    {
        public static T RemoveItem<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            var item = collection.FirstOrDefault(predicate);

            if (item != null) collection.Remove(item);

            return item;
        }

        public static ICollection<T> RemoveAll<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            foreach (var item in collection.Where(predicate).ToList())
            {
                collection.Remove(item);
            }

            return collection;
        }
    }
}