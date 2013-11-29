using System;
using System.Globalization;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.ExtensionMethods
{
    public static class DateTimeExtensions
    {
         public static string GetElapsedTime(this TimeSpan time)
         {
             if (time.TotalSeconds < 1.0)
                 return string.Format("{0}ms", time.Milliseconds);

             if (time.TotalMinutes < 1.0)
                 return string.Format("{0}s", time.Seconds);

             if (time.TotalHours < 1.0)
                 return string.Format("{0}m {1}s", time.Minutes, time.Seconds);

             return string.Format("{0}h {1}m {2}s", time.Hours, time.Minutes, time.Seconds);
         }

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