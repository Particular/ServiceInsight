using System.Reflection;

namespace NServiceBus.Profiler.Common.ExtensionMethods
{
    public static class ReflectionHelper
    {
         public static T GetAttribute<T>(this ICustomAttributeProvider provider, bool inherit = false) 
             where T : class 
         {
             var attrib = provider.GetCustomAttributes(typeof (T), inherit);
             if(attrib.Length > 0)
                 return (T)attrib[0];
             
             return null;
         }
    }
}