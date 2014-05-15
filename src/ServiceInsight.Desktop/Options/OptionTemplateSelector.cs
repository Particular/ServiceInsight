namespace Particular.ServiceInsight.Desktop.Options
{
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;

    public class OptionTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var option = item as OptionPropertyValue;
            if (option == null)
                return base.SelectTemplate(item, container);

            if (option.PropertyType == typeof(bool))
                return container.GetResource<DataTemplate>("CheckBoxTemplate");

            if (typeof(IList).IsAssignableFrom(option.PropertyType))
                return container.GetResource<DataTemplate>("ListTemplate");

            return container.GetResource<DataTemplate>("TextBoxTemplate");
        }
    }
}