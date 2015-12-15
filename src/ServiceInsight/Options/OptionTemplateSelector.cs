namespace ServiceInsight.Options
{
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;

    public class OptionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CheckBoxTemplate { get; set; }
        public DataTemplate ListTemplate { get; set; }
        public DataTemplate TextBoxTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var option = item as OptionPropertyValue;
            if (option == null)
                return base.SelectTemplate(item, container);

            if (option.PropertyType == typeof(bool))
                return CheckBoxTemplate;

            if (typeof(IList).IsAssignableFrom(option.PropertyType))
                return ListTemplate;

            return TextBoxTemplate;
        }
    }
}