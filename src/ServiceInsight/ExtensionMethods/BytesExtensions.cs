namespace ServiceInsight.ExtensionMethods
{
    using System.Linq;

    public static class BytesExtensions
    {
        public static bool StartsWith(this byte[] source, byte[] comparison) =>
            source != null &&
            comparison != null &&
            source.Length >= comparison.Length &&
            source.Take(comparison.Length).SequenceEqual(comparison);
    }
}