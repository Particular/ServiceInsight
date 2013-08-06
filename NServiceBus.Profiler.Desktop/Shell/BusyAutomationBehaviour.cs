using System.Windows;
using System.Windows.Automation;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public static class BusyAutomationBehaviour
    {
        public static readonly DependencyProperty IsApplicationBusyProperty;

        static BusyAutomationBehaviour()
        {
            IsApplicationBusyProperty = DependencyProperty.RegisterAttached("IsApplicationBusy", typeof (bool), typeof (BusyAutomationBehaviour), new PropertyMetadata(OnChanged));
        }

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutomationProperties.SetHelpText(d, GetIsApplicationBusy(d) ? "Busy" : string.Empty);
        }

        public static void SetIsApplicationBusy(DependencyObject element, bool value)
        {
            element.SetValue(IsApplicationBusyProperty, value);
        }

        public static bool GetIsApplicationBusy(DependencyObject element)
        {
            return (bool)element.GetValue(IsApplicationBusyProperty);
        }
    }
}