using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NServiceBus.Profiler.Desktop.Settings
{
    public class SettingBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}