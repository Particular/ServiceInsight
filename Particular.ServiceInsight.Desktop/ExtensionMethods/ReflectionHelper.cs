using System;
using System.ComponentModel;
using System.Reflection;

namespace Particular.ServiceInsight.Desktop.ExtensionMethods
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

         public static string GetDescription(this Enum value)
         {
             var field = value.GetType().GetField(value.ToString());
             var attribute = field.GetAttribute<DescriptionAttribute>();

             return attribute != null ? attribute.Description : value.ToString();
         }
    }
}