using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace Particular.ServiceInsight.Desktop.ExtensionMethods
{
    public static class GrammerExtensions
    {
        private static readonly PluralizationService Pluralizer;

        static GrammerExtensions()
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
    }
}