namespace ServiceInsight.MessageHeaders
{
    using System.Windows;
    using System.Windows.Controls;

    public class MessageHeaderValueTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ExceptionTemplate
        { get; set; }

        public DataTemplate TextTemplate
        { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is MessageHeaderKeyValue obj)
            {
                if (obj.Key.Contains("StackTrace"))
                {
                    return ExceptionTemplate;
                }
                return TextTemplate;
            }
            else
            {
                return base.SelectTemplate(item, container);
            }
        }
    }
}