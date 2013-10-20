using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NServiceBus.Profiler.Desktop.MessageList;
using NServiceBus.Profiler.Desktop.Shell;
using ActionFactory = Caliburn.PresentationFramework.Actions.Action;

namespace NServiceBus.Profiler.Desktop.ValueConverters
{
    public class ContextMenuActionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var model =  value as ContextMenuModel;
            if (model != null)
            {
                return string.Format("[Event ItemClick]=[Action {0}]", model.Name);
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}