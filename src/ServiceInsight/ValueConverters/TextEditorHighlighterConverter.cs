namespace Particular.ServiceInsight.Desktop.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using ICSharpCode.AvalonEdit.Highlighting;

    public class TextEditorHighlighterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var definitionName = value as string;
            
            if (definitionName != null)
            {
                return HighlightingManager.Instance.GetDefinition(definitionName);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var definition = value as IHighlightingDefinition;
            if (definition != null && targetType == typeof(string))
                return definition.Name;

            return value;
        }
    }
}