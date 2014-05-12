namespace Particular.ServiceInsight.Desktop.Settings
{
    using System.ComponentModel;

    public class SettingBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}