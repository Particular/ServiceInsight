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
    }
}