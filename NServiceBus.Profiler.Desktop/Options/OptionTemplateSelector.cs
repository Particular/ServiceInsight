using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Caliburn.PresentationFramework;

namespace NServiceBus.Profiler.Desktop.Options
{
    public class OptionTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var option = item as OptionPropertyValue;
            if (option == null)
                return base.SelectTemplate(item, container);

            if(option.PropertyType == typeof(bool))
                return container.GetResource<DataTemplate>("CheckBoxTemplate");

            if (typeof (IList).IsAssignableFrom(option.PropertyType))
                return container.GetResource<DataTemplate>("ListTemplate");

            return container.GetResource<DataTemplate>("TextBoxTemplate");
        }
    }
}