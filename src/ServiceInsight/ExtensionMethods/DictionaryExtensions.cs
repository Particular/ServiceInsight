namespace Particular.ServiceInsight.Desktop.ExtensionMethods
{
    using System.Collections.Generic;

    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
            where TKey : class
        {
            return key != null && dictionary.ContainsKey(key) ? dictionary[key] : defaultValue;
        }
    }
}