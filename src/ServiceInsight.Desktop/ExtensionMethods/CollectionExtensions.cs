namespace Particular.ServiceInsight.Desktop.ExtensionMethods
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
    }
}