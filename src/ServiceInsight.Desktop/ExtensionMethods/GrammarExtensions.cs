// ReSharper disable once CheckNamespace
namespace Particular.ServiceInsight.Desktop
{
    using System;
    using System.Data.Entity.Design.PluralizationServices;
    using System.Globalization;
    using Humanizer;

    public static class GrammarExtensions
    {
        static PluralizationService Pluralizer;

        static GrammarExtensions()
        {
            Pluralizer = PluralizationService.CreateService(new CultureInfo("en-US"));
        }

        public static string PluralizeWord(this string word, int count)
        {
            return string.Format("{0} {1}", count, (count == 1 ? Pluralizer.Singularize(word) : Pluralizer.Pluralize(word)));
        }

        public static string PluralizeVerb(this int count)
        {
            return string.Format("{0}", (count == 1 ? "is" : "are"));
        }

        public static string SubmillisecondHumanize(this TimeSpan timespan)
        {
            if (timespan.Ticks < 10000)
            {
                return String.Format("{0:0.##} milliseconds", timespan.Ticks / 10000.0);
            }

            return timespan.Humanize();
        }
    }
}