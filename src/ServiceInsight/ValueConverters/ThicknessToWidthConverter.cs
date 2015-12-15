using System.Windows;

namespace ServiceInsight.ValueConverters
{
	using System;
	using System.Windows.Data;

	public class ThicknessToWidthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var thickness = (Thickness)value;
			return thickness.Left;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}