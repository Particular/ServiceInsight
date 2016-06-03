namespace ServiceInsight.SequenceDiagram.Converter
{
    using System;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Data;

    public class SmartWrapConverter : IValueConverter
    {
        static readonly Regex PascalCaseWordPartsRegex;

        static SmartWrapConverter()
        {
            PascalCaseWordPartsRegex = new Regex(@"[A-Z]?[a-z]+|[0-9]+|[A-Z]+(?=[A-Z][a-z]|[0-9]|\b)",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = value as string;

            if (result == null)
            {
                return DependencyProperty.UnsetValue;
            }

            result = result.Replace(".", ".\u200B");
            result = result.Replace("-", "-\u200B");
            result = result.Replace("_", "_\u200B");

            return AddUnicodeZeroWidthSpaceBasedOnPascalCase(result);
        }

        static string AddUnicodeZeroWidthSpaceBasedOnPascalCase(string input)
        {
            var result = PascalCaseWordPartsRegex
                .Replace(input, match => "\u200B" + match.Value);

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}