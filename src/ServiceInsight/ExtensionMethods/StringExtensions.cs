namespace ServiceInsight.ExtensionMethods
{
    using System;

    public static class StringExtensions
    {
        public static bool IsEmpty(this string value) => string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);

        public static bool Contains(this string source, string toCheck, StringComparison comparison)
        {
            if (IsEmpty(source))
            {
                return true;
            }

            return source.IndexOf(toCheck, comparison) >= 0;
        }

        public static bool IsValidUrl(this string source)
        {
            if (Uri.TryCreate(source, UriKind.Absolute, out var result))
            {
                return result.Scheme == Uri.UriSchemeHttp ||
                       result.Scheme == Uri.UriSchemeHttps;
            }

            return false;
        }
    }
}