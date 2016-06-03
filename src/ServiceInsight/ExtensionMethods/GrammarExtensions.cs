// ReSharper disable once CheckNamespace
namespace ServiceInsight
{
    using System;
    using System.Data.Entity.Design.PluralizationServices;
    using System.Globalization;
    using Humanizer;

    public static class GrammarExtensions
    {
        static PluralizationService pluralizer;

        static GrammarExtensions()
        {
            pluralizer = PluralizationService.CreateService(new CultureInfo("en-US"));
        }

        public static string PluralizeWord(this string word, int count) => string.Format("{0} {1}", count, count == 1 ? pluralizer.Singularize(word) : pluralizer.Pluralize(word));

        public static string PluralizeVerb(this int count) => string.Format("{0}", count == 1 ? "is" : "are");

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