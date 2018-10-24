namespace ServiceInsight.ExtensionMethods
{
    using System.Linq;

    public static class EnumExtensions
    {
        public static bool In<T>(this T val, params T[] values) where T : struct
        {
            return values.Contains(val);
        }
    }
}