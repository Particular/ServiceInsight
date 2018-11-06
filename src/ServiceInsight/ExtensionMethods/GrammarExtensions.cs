// ReSharper disable once CheckNamespace
namespace ServiceInsight
{
    using System;
    using Humanizer;

    public static class GrammarExtensions
    {
        public static string SubmillisecondHumanize(this TimeSpan timespan)
        {
            if (timespan.Ticks < 10000)
            {
                return string.Format("{0:0.##} milliseconds", timespan.Ticks / 10000.0);
            }

            return timespan.Humanize();
        }
    }
}