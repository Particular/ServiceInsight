namespace Particular.ServiceInsight.Desktop.ExtensionMethods
{
    using System;
    using System.Collections.Generic;

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