namespace Particular.ServiceInsight.Desktop.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class BuilderExtensions
    {
        public static bool IsViewOrViewModel(this Type type)
        {
            return type != null &&
                   type.IsClass &&
                   !type.IsAbstract &&
                   type.Namespace != null &&
                   MatchingNames.Any(ns => type.Name.EndsWith(ns, StringComparison.InvariantCultureIgnoreCase));
        }

        static IEnumerable<string> MatchingNames
        {
            get
            {
                yield return "ViewModel";
                yield return "View";
            }
        }
    }
}