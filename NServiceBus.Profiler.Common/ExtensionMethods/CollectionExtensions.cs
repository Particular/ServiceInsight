using System;
using System.Collections.Generic;

namespace NServiceBus.Profiler.Common.ExtensionMethods
{
    public static class CollectionExtensions
    {
        public static void ForEach<T>(this IList<T> list, Action<T> action)
        {
            foreach(var item in list)
            {
                action(item);
            }
        }

        public static bool IsEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static void AddRange<T>(this IList<T> list, IList<T> collectionToAdd)
        {
            foreach (var item in collectionToAdd)
            {
                list.Add(item);
            }
        }
    }
}