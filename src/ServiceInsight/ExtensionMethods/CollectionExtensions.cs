namespace ServiceInsight.ExtensionMethods
{
    using System;
    using System.Collections.Generic;

    public static class CollectionExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> list, T item, IEqualityComparer<T> comparer = null)
        {
            var comp = comparer ?? EqualityComparer<T>.Default;

            var i = 0;
            foreach (var x in list)
            {
                if (comp.Equals(x, item))
                    return i;
                i++;
            }
            return -1;
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
                throw new ArgumentNullException("collection", "collection is null.");
            if (items == null)
                throw new ArgumentNullException("items", "items is null.");

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static IEnumerable<T> FullExcept<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer = null)
        {
            if (first == null)
                throw new ArgumentNullException("first", "first is null.");
            if (second == null)
                throw new ArgumentNullException("second", "second is null.");

            return InternalFullExcept(first, second, comparer ?? EqualityComparer<T>.Default);
        }

        private static IEnumerable<T> InternalFullExcept<T>(IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer)
        {
            var set = new HashSet<T>(second, comparer);
            foreach (var item in first)
            {
                if (!set.Remove(item))
                    yield return item;
            }
            foreach (var item in set)
            {
                yield return item;
            }
        }
    }
}