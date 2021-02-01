namespace ServiceInsight.ExtensionMethods
{
    using System;
    using System.Globalization;
    using Models;

    public static class DateTimeExtensions
    {
        public static DateTime? ParseHeaderDate(this string date)
        {
            if (DateTime.TryParseExact(date, HeaderInfo.MessageDateFormat, null, DateTimeStyles.None, out var result))
            {
                return result;
            }

            return null;
        }
    }
}