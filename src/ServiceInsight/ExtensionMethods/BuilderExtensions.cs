namespace Particular.ServiceInsight.Desktop.ExtensionMethods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Framework.Attachments;

    public static class BuilderExtensions
    {
        public static bool IsAttachment(this Type type)
        {
            return type != null &&
                   type.IsClass &&
                   !type.IsAbstract &&
                   typeof(IAttachment).IsAssignableFrom(type);
        }

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