using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ServiceInsight.Search
{
    public class OnlyDigitsValidationRule : ValidationRule
    {
        static readonly ValidationResult ValidResults = new ValidationResult(true, null);
        
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null)
            {
                var stringValue = value.ToString();
                var isNumber = int.TryParse(stringValue, out int number);

                if (!isNumber || number <= 0)
                {
                    return new ValidationResult(false, "Enter a valid page number.");
                }
            }

            return ValidResults;
        }
    }
}