using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Caliburn.PresentationFramework.Screens;

namespace NServiceBus.Profiler.Desktop.MessageProperties
{
    public class HeaderInfoTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var val = value;
            var properties = value.GetType()
                                  .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                  .Where(p => !PropertiesToExclude.Contains(p.Name))
                                  .Where(p => SupportedType.Any(x => x.IsAssignableFrom(p.PropertyType)))
                                  .ToList();

            var pds = properties.Select(x => new HeaderInfoPropertyDescriptor(x, value.GetType())).ToArray();
            var bp = base.GetProperties(context, value, attributes);

            return new PropertyDescriptorCollection(pds);
        }

        private static IEnumerable<Type> SupportedType
        {
            get
            {
                yield return typeof(IScreen);
                yield return typeof(string);
                yield return typeof(DateTime?);
            }
        }

        private static IEnumerable<string> PropertiesToExclude
        {
            get
            {
                yield return "DisplayName";
            }
        }
    }
}