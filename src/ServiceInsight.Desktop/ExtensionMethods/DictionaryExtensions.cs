using System.Collections.Generic;

namespace NServiceBus.Profiler.Desktop.ExtensionMethods
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
            where TKey : class
        {
            return key != null && dictionary.ContainsKey(key) ? dictionary[key] : defaultValue;
        }
    }
}