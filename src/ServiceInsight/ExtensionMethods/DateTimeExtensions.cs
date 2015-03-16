namespace Particular.ServiceInsight.Desktop.ExtensionMethods
{
    using System;
    using System.Globalization;
    using Models;

    public static class DateTimeExtensions
    {
        public static DateTime? ParseHeaderDate(this string date)
        {
            DateTime result;
            if (DateTime.TryParseExact(date, HeaderInfo.MessageDateFormat, null, DateTimeStyles.None, out result))
            {
                return result;
            }

            return null;
        }
    }
}