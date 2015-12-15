namespace ServiceInsight.ExtensionMethods
{
    using System;

    public static class StringExtensions
    {
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }

        public static bool Contains(this string source, string toCheck, StringComparison comparison)
        {
            if (IsEmpty(source))
                return true;

            return source.IndexOf(toCheck, comparison) >= 0;
        } 
    }
}